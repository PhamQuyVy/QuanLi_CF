using QuanLi_CF.Domain;
namespace QuanLi_CF.Events;
public delegate void OrderStatusChangedEventHandler(object sender, OrderStatusChangedEventArgs e);
public class OrderStatusChangedEventArgs : EventArgs
{
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }
    public OrderStatusChangedEventArgs(OrderStatus oldS, OrderStatus newS)
    {
        OldStatus = oldS;
        NewStatus = newS;
    }
}
