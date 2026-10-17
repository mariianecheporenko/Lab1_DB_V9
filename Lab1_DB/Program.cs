using MultiStoreLab;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string connectionString = "Host=localhost;Port=5432;Database=ecommerce_multistore_lab;Username=lab1_db;Password=lab1_db";

Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА #1 Варіант 9: Електронна комерція з кількома магазинами ===");
Console.WriteLine("Демонстрація роботи Unit of Work + Repository через Stored Procedures & Views\n");

using (var uow = new UnitOfWork(connectionString))
{
    try
    {
        Console.WriteLine("--- 1. Читання каталогу з View (v_store_catalog) ---");
        var catalog = await uow.Orders.GetStoreCatalogAsync(1);
        foreach (var item in catalog)
        {
            Console.WriteLine($"Товар: {item.ProductName} | SKU: {item.Sku} | Ціна: {item.Price} грн | Доступно: {item.AvailableQty} шт.");
        }

        Console.WriteLine("\n--- 2. Створення замовлення через процедуру (sp_create_order) ---");
        var itemsToOrder = new List<CreateOrderItemModel>
        {
            new CreateOrderItemModel { product_id = 1, quantity = 1 }
        };

        long newOrderId = await uow.Orders.CreateOrderAsync(
            userId: 2,
            storeId: 1,
            items: itemsToOrder,
            addressId: 1,
            shippingMethodId: 1,
            couponCode: "WELCOME10"
        );

        Console.WriteLine($"Успішно створено замовлення ID: {newOrderId}");

        Console.WriteLine("\n--- 3. Оплата замовлення через процедуру (sp_pay_order) ---");
        await uow.Orders.PayOrderAsync(newOrderId, "card", "TXN-CSHARP-9999", 2);
        Console.WriteLine("Замовлення успішно оплачено!");

        uow.Commit();

        Console.WriteLine("\n--- 4. Перевірка статусу з View (_order_details) ---");
        var orderDetails = await uow.Orders.GetOrderDetailsByIdAsync(newOrderId);
        if (orderDetails != null)
        {
            Console.WriteLine($"Замовлення #: {orderDetails.OrderNumber}");
            Console.WriteLine($"Статус: {orderDetails.Status}");
            Console.WriteLine($"Сума: {orderDetails.TotalAmount} грн");
            Console.WriteLine($"Клієнт: {orderDetails.CustomerEmail}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nПОМИЛКА: {ex.Message}");
        uow.Rollback();
    }
}

Console.WriteLine("\nРоботу програми завершено успішно!");