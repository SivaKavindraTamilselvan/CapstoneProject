using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IVendorOrderService
{
        public Task<ResponseGetOrderItems> UpdateTheOrderStatus(int orderItemId);

        public Task<List<ResponseGetOrderItems>> GetAllTheActiveOrder(int vendorId);
}