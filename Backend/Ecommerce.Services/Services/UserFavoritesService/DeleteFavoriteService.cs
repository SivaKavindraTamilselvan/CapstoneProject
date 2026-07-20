using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<ResponseFavoriteItemsDTO> DeleteFavorite(RequestDeleteFavoriteItemsDTO requestDeleteFavoriteItemsDTO, int userId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _userValidation.ValidateUser(userId);
            _logger.LogInformation("Deleting favorite item. FavoritesItemsId: {FavoritesItemsId}", requestDeleteFavoriteItemsDTO.FavoritesItemsId);

            var favoritesItems = await _favoriteValidation.ValidateFavoriteItems(requestDeleteFavoriteItemsDTO.FavoritesItemsId);
            _logger.LogInformation("FavoritesItemsId {FavoritesItemsId} validated. FavoritesId {FavoritesId}, ProductVariantId {ProductVariantId}", favoritesItems.FavoritesItemsId, favoritesItems.FavoritesId, favoritesItems.ProductVariantId);

            var deleted = await _favoriteItemsRepsository.Delete(favoritesItems.FavoritesItemsId);
            if (deleted == null)
            {
                _logger.LogError("Failed to delete FavoritesItemsId {FavoritesItemsId}", favoritesItems.FavoritesItemsId);
                throw new DataRegistrationException("Favorite item deletion failed");
            }
            _logger.LogInformation("Deleted favorite item. FavoritesItemsId: {FavoritesItemsId}, FavoritesId: {FavoritesId}, ProductVariantId: {ProductVariantId}", favoritesItems.FavoritesItemsId, favoritesItems.FavoritesId, favoritesItems.ProductVariantId);

            var favoriteLog = new LogChanges
            {
                TableName = nameof(FavoritesItems),
                RecordId = favoritesItems.FavoritesItemsId,
                Actions = (int)AuditAction.Deleted,
                OldValue = $"FavoritesItemsId={favoritesItems.FavoritesItemsId}, FavoritesId={favoritesItems.FavoritesId}, ProductVariantId={favoritesItems.ProductVariantId}",
                NewValue = string.Empty,
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
            _logger.LogInformation("Transaction committed successfully for FavoritesItemsId {FavoritesItemsId}", favoritesItems.FavoritesItemsId);

            return _mapper.Map<ResponseFavoriteItemsDTO>(favoritesItems);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while deleting FavoritesItemsId {FavoritesItemsId} for UserId {UserId}", requestDeleteFavoriteItemsDTO.FavoritesItemsId, userId);
            _logger.LogInformation("Transaction rolled back while deleting FavoritesItemsId {FavoritesItemsId} for UserId {UserId}", requestDeleteFavoriteItemsDTO.FavoritesItemsId, userId);
            throw;
        }
    }
}