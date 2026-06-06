using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class ReviewService : IReviewService
{
    private readonly IReviewRepsository _reviewRepsository;
    private readonly IOrderValidation _orderValidation;
    private readonly IMapper _mapper;
    public ReviewService(IReviewRepsository reviewRepsository, IMapper mapper, IOrderValidation orderValidation)
    {
        _reviewRepsository = reviewRepsository;
        _orderValidation = orderValidation;
        _mapper = mapper;
    }
    public async Task<ResponseAddReviewDTO> AddReview(RequestAddReviewDTO requestAddReviewDTO)
    {
        var order = await _orderValidation.ValidateOrderItem(requestAddReviewDTO.OrderDetailsId);
        if (order.OrderItemStatusId != 4)
        {
            throw new DataApprovalStatusException("Order Item Not Approved Yet");
        }
        var review = _mapper.Map<Reviews>(requestAddReviewDTO);
        var createdReview = await _reviewRepsository.Create(review);
        if (createdReview == null)
        {
            throw new DataNotFoundException("Review Not Created");
        }
        return _mapper.Map<ResponseAddReviewDTO>(createdReview);
    }
}