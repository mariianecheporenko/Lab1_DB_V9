using Dapper;
using System.Data;
using System.Text.Json;

namespace MultiStoreLab;

public interface IOrderRepository
{
    Task<IEnumerable<StoreCatalogDto>> GetStoreCatalogAsync(long storeId);
    Task<OrderDetailsDto?> GetOrderDetailsByIdAsync(long orderId);
    Task<long> CreateOrderAsync(long userId, long storeId, List<CreateOrderItemModel> items, long addressId, int shippingMethodId, string? couponCode = null);
    Task PayOrderAsync(long orderId, string paymentMethod, string transactionId, long userId);
}