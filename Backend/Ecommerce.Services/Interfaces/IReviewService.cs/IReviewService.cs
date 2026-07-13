using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;
public interface IReviewService
{
        public  Task<ProductReviewSummaryDTO> GetProductReviewSummary(int productId);
    public Task<ResponseAddReviewDTO> AddReview(RequestAddReviewDTO requestAddReviewDTO);
}