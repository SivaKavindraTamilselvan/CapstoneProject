using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

public partial class UserOrderService : IUserOrderService
{
    private readonly ICartItemsRepsository _cartItemsRepsository;
    private readonly IUserCouponService _userCouponService;
    private readonly IAddressRepsository _addressRepsository;
    private readonly IOrderRepsository _orderRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IUserCartService _userCartService;
    private readonly IShipRocketService _shipRocketService;
    private readonly IShipmentRepsository _shipmentRepository;
    private readonly IShipmentItemsRepsository _shipmentItemsRepository;
    private readonly IShipmentTrackingRepsository _shipmentTrackingRepository;
    private readonly IMapper _mapper;
    private class SelectedCartInventory
    {
        public CartItems CartItem { get; set; } = null!;
        public Inventory Inventory { get; set; } = null!;
        public decimal ShippingRate { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
    }
    public UserOrderService(ICartItemsRepsository cartItemsRepsository, IShipRocketService shipRocketService, IUserCartService userCartService, IUserCouponService userCouponService, IAddressRepsository addressRepsository, IOrderRepsository orderRepsository, IOrderItemRepsository orderItemRepsository, IMapper mapper, IShipmentItemsRepsository shipmentItemsRepsository, IShipmentRepsository shipmentRepsository, IShipmentTrackingRepsository shipmentTrackingRepsository)
    {
        _cartItemsRepsository = cartItemsRepsository;
        _userCouponService = userCouponService;
        _addressRepsository = addressRepsository;
        _orderRepsository = orderRepsository;
        _orderItemRepsository = orderItemRepsository;
        _userCartService = userCartService;
        _shipRocketService = shipRocketService;
        _shipmentItemsRepository = shipmentItemsRepsository;
        _shipmentRepository = shipmentRepsository;
        _shipmentTrackingRepository = shipmentTrackingRepsository;
        _mapper = mapper;
    }

    private async Task<List<SelectedCartInventory>> GetTheInventoryPickupAddress(List<CartItems> cartItems, Address address, int cod)
    {
        var selectedItems = new List<SelectedCartInventory>();
        foreach (var list in cartItems)
        {
            Console.WriteLine(list.ProductVariant.Inventories.FirstOrDefault().AvailableQuantity);
            var inventories = list.ProductVariant!.Inventories.Where(i => i.AvailableQuantity >= list.Qunatity).ToList();
            Inventory? bestInventory = null;
            decimal bestRate = decimal.MaxValue;
            int bestEdd = 0;

            foreach (var inventory in inventories)
            {
                var request = new ServiceabilityRequestDTO
                {
                    PickupPostcode = inventory.Address!.PinCode,
                    DeliveryPostcode = address.PinCode,
                    Weight = list.Qunatity * list.ProductVariant.WeightInKgs,
                    Cod = cod
                };
                var courier = await _shipRocketService.CheckServiceability(request);

                if (courier != null && courier.Rate < bestRate)
                {
                    bestRate = courier.Rate;
                    bestInventory = inventory;
                    bestEdd = int.Parse(courier.EstimatedDeliveryDays);
                }

            }
            if (bestInventory == null)
            {
                throw new DataNotFoundException("No courier available for product");
            }
            selectedItems.Add(new SelectedCartInventory
            {
                CartItem = list,
                Inventory = bestInventory,
                ShippingRate = bestRate,
                EstimatedDeliveryDays = bestEdd,
                ExpectedDeliveryDate = DateTime.Now.AddDays(2 + bestEdd)
            });
        }
        return selectedItems;
    }

    private async Task<decimal> CalculateShippingCharge(Address pickup, Address delivery, decimal weight, int cod)
    {
        ServiceabilityRequestDTO serviceabilityRequestDTO = new ServiceabilityRequestDTO();
        serviceabilityRequestDTO.Cod = cod;
        serviceabilityRequestDTO.DeliveryPostcode = delivery.PinCode;
        serviceabilityRequestDTO.Weight = weight;
        serviceabilityRequestDTO.PickupPostcode = pickup.PinCode;
        var bestCourier = await _shipRocketService.CheckServiceability(serviceabilityRequestDTO);
        if (bestCourier == null)
        {
            throw new Exception("No courier available");
        }
        return bestCourier.Rate;

    }
    private async Task<List<CartItems>> ValidateProduct(int userId)
    {
        var cartItems = await _cartItemsRepsository.GetCartItemsByUserId(userId);
        if (cartItems.Count == 0)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        if (cartItems.Any(c => c.ProductVariant == null))
        {
            throw new DataNotFoundException("Product variant not loaded");
        }
        return cartItems;
    }
    private async Task<Address> ValidateAddress(int addressId)
    {
        var address = await _addressRepsository.Get(addressId);
        if (address == null)
        {
            throw new DataNotFoundException("Address Not Found");
        }
        return address;
    }
    private async Task<Coupons> ValidateCoupon(int userId, int couponId)
    {
        var validateCoupon = await _userCouponService.GetAllAvailableCoupons(userId);
        Coupons? selectedCoupon = null;

        selectedCoupon = validateCoupon.FirstOrDefault(c => c.CouponId == couponId);
        if (selectedCoupon == null)
        {
            throw new DataNotFoundException("The Coupon Is Not Available For You");
        }
        return selectedCoupon;

    }
    private decimal CalculateProductCharge(List<CartItems> cartItems)
    {
        decimal productCharge = cartItems.Sum(c => c.Qunatity * c.ProductVariant!.Price);
        return productCharge;
    }
    private decimal CalculateOrderWeight(List<CartItems> cartItems)
    {
        decimal Weight = cartItems.Sum(c => c.Qunatity * c.ProductVariant!.WeightInKgs);
        return Weight;
    }
    private async Task<Order> CreateOrder(decimal prductCharge, decimal shippingCharge, decimal couponCharge, int userId, int addressId)
    {
        Order order = new Order();
        order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";
        order.UserId = userId;
        order.TotalProductAmount = prductCharge;
        order.TotalCouponAmount = couponCharge;
        order.TotalShippingAmount = shippingCharge;
        order.FinalAmount = order.TotalProductAmount - order.TotalCouponAmount + order.TotalShippingAmount;
        order.AddressId = addressId;
        order.OrderDate = DateTime.Now;
        order.OrderStatusId = 1;
        var createdOrder = await _orderRepsository.Create(order);
        if (createdOrder == null)
        {
            throw new DataNotFoundException("Order Not Created");
        }
        return createdOrder;
    }
    private async Task<List<OrderItems>> CreateOrderItems(List<SelectedCartInventory> selectedItems, Order order, Coupons? selectedCoupon)
    {
        List<OrderItems> orderItemsList = new List<OrderItems>();

        foreach (var selected in selectedItems)
        {
            var cartItem = selected.CartItem;

            OrderItems orderItems = new OrderItems();
            orderItems.ProductVariantId = cartItem.ProductVariantId;
            orderItems.InventoryId = selected.Inventory.InventoryId;
            orderItems.Quantity = cartItem.Qunatity;
            orderItems.OrderId = order.OrderId;
            orderItems.UnitPrice = cartItem.ProductVariant!.Price;
            orderItems.Discount = 0;
            orderItems.OrderItemStatusId = 1;

            if (selectedCoupon != null)
            {
                bool applicable = selectedCoupon.CouponsProducts
                    .Any(p => p.ProductId == cartItem.ProductVariant!.ProductId);

                if (applicable)
                {
                    orderItems.Discount = selectedCoupon.DiscountValue;
                }
            }

            var createdOrderItem = await _orderItemRepsository.Create(orderItems);

            if (createdOrderItem != null)
            {
                orderItemsList.Add(createdOrderItem);
            }
        }

        return orderItemsList;
    }
    public async Task<ResponseAddOrderDTO> AddOrder(RequestAddOrderDTO requestAddOrderDTO, int userId)
    {
        var cartItems = await ValidateProduct(userId);

        var deliveryAddress = await ValidateAddress(requestAddOrderDTO.AddressId);

        decimal productCharge = CalculateProductCharge(cartItems);

        int cod = requestAddOrderDTO.PaymentMethod == 1 ? 1 : 0;

        Coupons? selectedCoupon = null;

        if (requestAddOrderDTO.CouponId != null)
        {
            selectedCoupon = await ValidateCoupon(userId, requestAddOrderDTO.CouponId.Value);
        }

        decimal couponCharge = selectedCoupon?.DiscountValue ?? 0;

        var selectedItems = await GetTheInventoryPickupAddress(
            cartItems,
            deliveryAddress,
            cod);

        var groupedShipments = selectedItems.GroupBy(x => new
        {
            VendorId = x.CartItem.ProductVariant!.Product!.VendorId,
            PickupAddressId = x.Inventory.AddressId
        }).ToList();

        decimal totalShippingCharge = 0;

        var shipmentChargeDetails = new List<(int PickupAddressId, decimal ShippingCharge)>();

        foreach (var group in groupedShipments)
        {
            var pickupAddress = group.First().Inventory.Address!;

            decimal shipmentWeight = group.Sum(x =>
                x.CartItem.Qunatity *
                x.CartItem.ProductVariant!.WeightInKgs);

            decimal shippingCharge = await CalculateShippingCharge(
                pickupAddress,
                deliveryAddress,
                shipmentWeight,
                cod);

            totalShippingCharge += shippingCharge;

            shipmentChargeDetails.Add((
                group.Key.PickupAddressId,
                shippingCharge
            ));
        }

        var order = await CreateOrder(
            productCharge,
            totalShippingCharge,
            couponCharge,
            userId,
            deliveryAddress.AddressId);

        var orderItems = await CreateOrderItems(
            selectedItems,
            order,
            selectedCoupon);

        foreach (var group in groupedShipments)
        {
            var pickupAddress = group.First().Inventory.Address!;

            var shippingCharge = shipmentChargeDetails
                .First(x => x.PickupAddressId == group.Key.PickupAddressId)
                .ShippingCharge;
            var ExpectedDeliveryDate = group.First().ExpectedDeliveryDate;

            var shipment = await CreateShipment(
                order,
                pickupAddress,
                shippingCharge,ExpectedDeliveryDate);

            foreach (var selected in group)
            {
                var orderItem = orderItems.First(oi =>
                    oi.ProductVariantId == selected.CartItem.ProductVariantId &&
                    oi.InventoryId == selected.Inventory.InventoryId);

                await _shipmentItemsRepository.Create(new ShipmentItems
                {
                    ShipmentId = shipment.ShipmentId,
                    OrderItemsId = orderItem.OrderItemsId
                });
            }
            await CreateShipmentTracking(shipment);
        }

        await _userCartService.DeleteAllCart(userId);

        return _mapper.Map<ResponseAddOrderDTO>(order);
    }
    private async Task<Shipment> CreateShipment(Order order, Address pickupAddress, decimal shippingCharge,DateTime dateTime)
    {
        Shipment shipment = new Shipment
        {
            OrderId = order.OrderId,
            PickupAddressId = pickupAddress.AddressId,
            ShippingCharge = shippingCharge,
            ShipmentStatusId = 1, // Pending
            TrackingNumber = null,
            CreatedAt = DateTime.Now,
            ExpectedDeliveryDate = dateTime
        };

        var createdShipment = await _shipmentRepository.Create(shipment);

        if (createdShipment == null)
        {
            throw new DataNotFoundException("Shipment not created");
        }

        return createdShipment;
    }
    private async Task CreateShipmentTracking(Shipment shipment)
    {
        var tracking = new ShipmentTracking
        {
            ShipmentId = shipment.ShipmentId,
            ShipmentStatusId = shipment.ShipmentStatusId,
            Location = "Warehouse",
            Remarks = "Shipment created successfully",
            UpdatedAt = DateTime.Now
        };

        await _shipmentTrackingRepository.Create(tracking);
    }
}