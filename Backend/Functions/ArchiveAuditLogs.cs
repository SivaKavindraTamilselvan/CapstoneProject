using System.Reflection;
using System.Text;
using System.IO.Compression;
using Azure.Storage.Blobs;
using Ecommerce.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Functions;

public class ArchiveAuditLogs
{
    private readonly EcommerceContext _context;
    private readonly IConfiguration _configuration;

    public ArchiveAuditLogs(EcommerceContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [Function("ArchiveAuditLogs")]
    public async Task Run([TimerTrigger("0 5 4 * * *")] TimerInfo timer, FunctionContext context)
    {
        var logger = context.GetLogger("ArchiveAuditLogs");

        // 1. Discover every DbSet<T> on the context via reflection
        var dbSetProperties = typeof(EcommerceContext)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .ToList();

        if (!dbSetProperties.Any())
        {
            logger.LogWarning("No DbSet properties found on EcommerceContext.");
            return;
        }

        // 2. Build a zip in memory containing one CSV per table
        using var zipStream = new MemoryStream();
        var tableCount = 0;
        var totalRows = 0;

        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var dbSetProp in dbSetProperties)
            {
                var entityType = dbSetProp.PropertyType.GetGenericArguments()[0];
                var tableName = dbSetProp.Name;

                var rows = await GetAllRowsAsync(entityType);
                if (rows.Count == 0)
                {
                    logger.LogInformation($"{tableName}: no rows, skipping.");
                    continue;
                }

                var csv = BuildCsv(entityType, rows);

                var entry = archive.CreateEntry($"{tableName}.csv");
                using (var entryStream = entry.Open())
                using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                {
                    await writer.WriteAsync(csv);
                }

                tableCount++;
                totalRows += rows.Count;
                logger.LogInformation($"{tableName}: exported {rows.Count} row(s).");
            }
        }

        zipStream.Position = 0;

        if (tableCount == 0)
        {
            logger.LogInformation("No data found in any table — nothing to archive.");
            return;
        }

        // 3. Upload the zip to Blob Storage
        var connectionString = _configuration["BlobStorage:ConnectionString"];
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient("audit-log-archives");
        await containerClient.CreateIfNotExistsAsync();

        var blobName = $"full-db-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip";
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(zipStream, overwrite: true);

        // 4. Verify upload landed before considering the run complete
        var properties = await blobClient.GetPropertiesAsync();
        if (properties.Value.ContentLength == 0)
        {
            logger.LogError($"Blob {blobName} uploaded but is empty.");
            return;
        }

        logger.LogInformation(
            $"Exported {tableCount} table(s), {totalRows} total row(s), to blob: {blobName} ({properties.Value.ContentLength} bytes)");
    }

    // Fetch all rows for a given entity type using reflection, since we don't
    // know the type at compile time here
    private async Task<List<object>> GetAllRowsAsync(Type entityType)
    {
        var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
            .MakeGenericMethod(entityType);
        var dbSet = setMethod.Invoke(_context, null)!;

        // Build and invoke: dbSet.AsNoTracking().ToListAsync()
        var asNoTrackingMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethod(nameof(EntityFrameworkQueryableExtensions.AsNoTracking))!
            .MakeGenericMethod(entityType);
        var noTrackingSet = asNoTrackingMethod.Invoke(null, new[] { dbSet })!;

        var toListAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethod(nameof(EntityFrameworkQueryableExtensions.ToListAsync))!
            .MakeGenericMethod(entityType);

        var task = (Task)toListAsyncMethod.Invoke(null, new[] { noTrackingSet, CancellationToken.None })!;
        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var list = (System.Collections.IList)resultProperty.GetValue(task)!;

        return list.Cast<object>().ToList();
    }

    private static string BuildCsv(Type entityType, List<object> items)
    {
        var sb = new StringBuilder();

        var properties = entityType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsSimpleType(p.PropertyType))
            .ToArray();

        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        foreach (var item in items)
        {
            var values = properties.Select(p => EscapeCsv(FormatValue(p.GetValue(item))));
            sb.AppendLine(string.Join(",", values));
        }

        return sb.ToString();
    }

    private static bool IsSimpleType(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        return underlying.IsPrimitive
            || underlying.IsEnum
            || underlying == typeof(string)
            || underlying == typeof(decimal)
            || underlying == typeof(DateTime)
            || underlying == typeof(DateTimeOffset)
            || underlying == typeof(Guid)
            || underlying == typeof(TimeSpan);
    }

    private static string FormatValue(object? value)
    {
        if (value is null) return "";
        if (value is DateTime dt) return dt.ToString("o");
        if (value is DateTimeOffset dto) return dto.ToString("o");
        return value.ToString() ?? "";
    }

    private static string EscapeCsv(string field)
    {
        if (string.IsNullOrEmpty(field)) return "";
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}