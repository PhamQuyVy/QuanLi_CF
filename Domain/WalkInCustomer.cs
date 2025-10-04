namespace QuanLi_CF.Domain;

public class WalkInCustomer : Customer
{
    public override decimal GetDiscountPercent(Order order) => 0m;
}