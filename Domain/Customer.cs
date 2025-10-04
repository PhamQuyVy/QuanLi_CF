namespace QuanLi_CF.Domain;
public abstract class Customer
{
    public string CustomerID { get; } = Guid.NewGuid().ToString();
    public string fullName { get; set; } = "";
    public string Phone { get; set; }
    public abstract decimal GetDiscountPercent(Order order);

}