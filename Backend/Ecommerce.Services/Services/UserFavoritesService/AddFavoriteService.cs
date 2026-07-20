using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<ResponseFavoriteItemsDTO> AddFavorite(RequestAddFavoriteItemsDTO requestAddFavoriteItemsDTO, int userId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Adding favorite item. UserId: {UserId}, ProductVariantId: {ProductVariantId}", userId, requestAddFavoriteItemsDTO.ProductVariantId);

            await _userValidation.ValidateUser(userId);
            var favorite = await _favoriteValidation.ValidateFavoriteByUserId(userId);
            _logger.LogInformation("Favorites list validated for UserId {UserId}, FavoritesId {FavoritesId}", userId, favorite.FavoritesId);

            await _favoriteValidation.ValidateFavoriteItemsByProductAndUser(requestAddFavoriteItemsDTO.ProductVariantId, favorite.FavoritesId);
            _logger.LogInformation("ProductVariantId {ProductVariantId} confirmed not already favorited for FavoritesId {FavoritesId}", requestAddFavoriteItemsDTO.ProductVariantId, favorite.FavoritesId);

            FavoritesItems favoritesItems = new FavoritesItems
            {
                FavoritesId = favorite.FavoritesId,
                ProductVariantId = requestAddFavoriteItemsDTO.ProductVariantId
            };

            var createdFavoriteItem = await _favoriteItemsRepsository.Create(favoritesItems);
            if (createdFavoriteItem == null)
            {
                _logger.LogError("Failed to create FavoritesItem for UserId {UserId}, ProductVariantId {ProductVariantId}", userId, requestAddFavoriteItemsDTO.ProductVariantId);
                throw new DataRegistrationException("Favorite item creation failed");
            }
            _logger.LogInformation("Added favorite item. FavoritesItemId: {FavoritesItemId}, FavoritesId: {FavoritesId}, ProductVariantId: {ProductVariantId}", createdFavoriteItem.FavoritesItemsId, createdFavoriteItem.FavoritesId, createdFavoriteItem.ProductVariantId);

            var favoriteLog = new LogChanges
            {
                TableName = nameof(FavoritesItems),
                RecordId = createdFavoriteItem.FavoritesItemsId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"FavoritesItemsId={createdFavoriteItem.FavoritesItemsId}, FavoritesId={createdFavoriteItem.FavoritesId}, ProductVariantId={createdFavoriteItem.ProductVariantId}",
                UserId = userId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(favoriteLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", favoriteLog.TableName, favoriteLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", favoriteLog.TableName, favoriteLog.RecordId);

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for FavoritesItemId {FavoritesItemId}", createdFavoriteItem.FavoritesItemsId);

            return _mapper.Map<ResponseFavoriteItemsDTO>(createdFavoriteItem);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while adding favorite item for UserId {UserId}, ProductVariantId {ProductVariantId}", userId, requestAddFavoriteItemsDTO.ProductVariantId);
            _logger.LogInformation("Transaction rolled back while adding favorite item for UserId {UserId}, ProductVariantId {ProductVariantId}", userId, requestAddFavoriteItemsDTO.ProductVariantId);
            throw;
        }
    }
}