namespace QuanLi_CF.Domain;

public class OrderLine
{
    public DrinkProduct product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineDiscountPercent { get; set; }
    public List<ToppingItem> Topping { get; } = new();
    public OrderLine(DrinkProduct product, int quantity, decimal unitPrice, decimal lineDiscountPercent = 0m)
    {
        this.product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
        LineDiscountPercent = lineDiscountPercent;
    }
    public decimal ToppingTotal
    {
        get
        {
            return Topping.Sum(t => t.Price);
        }
    }
    public decimal LineAmount
    {
        get
        {
            return (Quantity * UnitPrice + ToppingTotal) * (1 - LineDiscountPercent);
        }
    }
    public override string ToString()
    {
        return $"{product.Name} - {Quantity} x {UnitPrice} VND - Line Discount: {LineDiscountPercent:P} - Topping: {ToppingTotal} VND - Line Amount: {LineAmount} VND";
    }
}