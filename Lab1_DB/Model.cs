namespace MultiStoreLab;

public class StoreCatalogDto
{
    public long StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public long ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int AvailableQty { get; set; }
}

public class OrderDetailsDto
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PlacedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
}

public class CreateOrderItemModel
{
    public long product_id { get; set; }
    public int quantity { get; set; }
}