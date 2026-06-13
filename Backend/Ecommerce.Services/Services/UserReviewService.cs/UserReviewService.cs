using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class ReviewService : IReviewService
{
    private readonly ILogger<ReviewService> _logger;
    private readonly IReviewRepsository _reviewRepsository;
    private readonly IOrderValidation _orderValidation;
    private readonly IMapper _mapper;
    public ReviewService(ILogger<ReviewService> logger,IReviewRepsository reviewRepsository, IMapper mapper, IOrderValidation orderValidation)
    {
        _reviewRepsository = reviewRepsository;
        _orderValidation = orderValidation;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<ResponseAddReviewDTO> AddReview(RequestAddReviewDTO requestAddReviewDTO)
    {
        _logger.LogInformation("Adding review for OrderItemId {OrderItemId}",requestAddReviewDTO.OrderDetailsId);
        var order = await _orderValidation.ValidateOrderItem(requestAddReviewDTO.OrderDetailsId);
        if (order.OrderItemStatusId != 4)
        {
            _logger.LogWarning("Review cannot be added for OrderItemId {OrderItemId} because status is {OrderItemStatusId}",requestAddReviewDTO.OrderDetailsId,order.OrderItemStatusId);
            throw new DataApprovalStatusException("Order Item Not Delivered Yet");
        }
        var review = _mapper.Map<Reviews>(requestAddReviewDTO);
        var createdReview = await _reviewRepsository.Create(review);
        if (createdReview == null)
        {
            _logger.LogWarning("Review creation failed for OrderItemId {OrderItemId}",requestAddReviewDTO.OrderDetailsId);
            throw new DataNotFoundException("Review Not Created");
        }
        _logger.LogInformation("ReviewId {ReviewId} created successfully for OrderItemId {OrderItemId}",createdReview.ReviewId,requestAddReviewDTO.OrderDetailsId);
        return _mapper.Map<ResponseAddReviewDTO>(createdReview);
    }
}