using QuanLi_CF.Events;
using QuanLi_CF.Interface;

namespace QuanLi_CF.Domain;

public class Order
{
    public string OrderNo { get; } = DateTime.Now.Ticks.ToString();
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public Customer Customer { get; }
    public decimal Subtotal { get; private set; }
    public decimal CustomerDiscount { get; private set; }
    public decimal OrderDiscount { get; private set; }
    public decimal VatRate { get; } = 0.08m;  
    public decimal VatAmount { get; private set; }  
    public decimal Total { get; private set; }
    public List<OrderLine> Lines { get; set; } = new();

    public OrderStatus Status { get; private set; } = OrderStatus.Draft;
    public event EventHandler<OrderStatusChangedEventArgs> StatusChanged = delegate { };
    private readonly IPriceRule priceRule;
    private readonly IOrderDiscountPolicy orderPolicy;
    public Order(Customer customer, IPriceRule pr, IOrderDiscountPolicy op)
    {
        Customer = customer;
        priceRule = pr;
        orderPolicy = op;
    }
    public void AddLine(OrderLine line) => Lines.Add(line);
    public void RecalcTotals()
    {
        Subtotal = Lines.Sum(l => priceRule.ComputeLineAmount(l));
        CustomerDiscount = Subtotal * Customer.GetDiscountPercent(this);
        OrderDiscount = orderPolicy.ComputeOrderLevelDiscount(this);
        var taxable = Subtotal - CustomerDiscount - OrderDiscount;
        VatAmount = taxable * VatRate;
        Total = taxable + VatAmount;
    }
    public void ChangeStatus(OrderStatus s)
    {
        var old = Status;
        Status = s;
        StatusChanged(this, new OrderStatusChangedEventArgs(old, s));
    }
    public override string ToString()
    {
        return $"Order No: {OrderNo} - Date: {OrderDate} - Customer: {Customer.fullName} - " +
               $"Subtotal: {Subtotal} VND - Customer Discount: {CustomerDiscount} VND - " +
               $"Order Discount: {OrderDiscount} VND - VAT({VatRate:P0}): {VatAmount} VND - " +
               $"Total: {Total} VND";
    }
}
