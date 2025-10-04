namespace QuanLi_CF.Domain;

public class DrinkProduct
{
    public string ProductID { get; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public decimal BasePrice { get; set; }
    public int StockQty { get; set; }
    public Size size { get; set; }
    public bool IsActive { get; set; } = true;
    public override string ToString()
    {
        return $"{Name} - {size}- {BasePrice} VND - Stock: {StockQty} - Active: {IsActive}";
    }
}