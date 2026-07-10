using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;
public interface IReviewService
{
    public Task<ResponseAddReviewDTO> AddReview(RequestAddReviewDTO requestAddReviewDTO);
}