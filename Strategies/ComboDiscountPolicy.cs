using QuanLi_CF.Domain;
using QuanLi_CF.Interface;

namespace QuanLi_CF.Strategies;

public class ComboDiscountPolicy : IOrderDiscountPolicy
{
    public decimal ComputeOrderLevelDiscount(Order order)
    {
        var largeQty = order.Lines.Where(l => l.product.size == Size.Large).Sum(l => l.Quantity);

        if (largeQty >= 3)
            return order.Subtotal * 0.05m;
        else
            return 0m;
    }
}
