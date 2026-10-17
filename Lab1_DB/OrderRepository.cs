using Dapper;
using MultiStoreLab;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;

namespace MultiStoreLab
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnection _connection;
        private readonly Func<IDbTransaction> _getTransaction;

        public OrderRepository(IDbConnection connection, Func<IDbTransaction> getTransaction)
        {
            _connection = connection;
            _getTransaction = getTransaction;
        }

        public async Task<IEnumerable<StoreCatalogDto>> GetStoreCatalogAsync(long storeId)
        {
            const string sql = """
                SELECT store_id AS "StoreId",
                       store_name AS "StoreName",
                       product_id AS "ProductId",
                       sku AS "Sku",
                       product_name AS "ProductName",
                       price AS "Price",
                       available_qty AS "AvailableQty"
                  FROM v_store_catalog
                 WHERE store_id = @StoreId;
                """;
            return await _connection.QueryAsync<StoreCatalogDto>(sql, new { StoreId = storeId }, transaction: _getTransaction());
        }

        public async Task<OrderDetailsDto?> GetOrderDetailsByIdAsync(long orderId)
        {
            const string sql = """
                SELECT id AS "Id",
                       order_number AS "OrderNumber",
                       status AS "Status",
                       placed_at AS "PlacedAt",
                       total_amount AS "TotalAmount",
                       customer_email AS "CustomerEmail",
                       store_name AS "StoreName"
                  FROM v_order_details
                 WHERE id = @OrderId;
                """;
            return await _connection.QueryFirstOrDefaultAsync<OrderDetailsDto>(sql, new { OrderId = orderId }, transaction: _getTransaction());
        }

        public async Task<long> CreateOrderAsync(long userId, long storeId, List<CreateOrderItemModel> items, long addressId, int shippingMethodId, string? couponCode = null)
        {
            var itemsJson = JsonSerializer.Serialize(items);
            var p = new DynamicParameters();
            p.Add("p_user_id", userId);
            p.Add("p_store_id", storeId);
            p.Add("p_items", itemsJson, DbType.Object); // JSONB
            p.Add("p_address_id", addressId);
            p.Add("p_shipping_method_id", shippingMethodId);
            p.Add("p_coupon_code", couponCode);
            p.Add("p_payment_method", "card");
            p.Add("p_order_id", dbType: DbType.Int64, direction: ParameterDirection.InputOutput);

            await _connection.ExecuteAsync("CALL sp_create_order(@p_user_id, @p_store_id, @p_items::jsonb, @p_address_id, @p_shipping_method_id, @p_coupon_code, @p_payment_method::payment_method, @p_order_id);", p, transaction: _getTransaction());

            return p.Get<long>("p_order_id");
        }

        public async Task PayOrderAsync(long orderId, string paymentMethod, string transactionId, long userId)
        {
            const string sql = "CALL sp_pay_order(@OrderId, @PaymentMethod::payment_method, @TransactionId, @UserId);";
            await _connection.ExecuteAsync(sql, new
            {
                OrderId = orderId,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId,
                UserId = userId
            }, transaction: _getTransaction());
        }
    }

}
