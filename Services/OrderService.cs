using QuanLi_CF.Domain;
using QuanLi_CF.Events;
using QuanLi_CF.Exceptions;
using QuanLi_CF.Interface;
namespace QuanLi_CF.Services;

public class OrderService
{
    private readonly InventoryService inventory;
    private readonly IRepository<Order, string> repo;
    public event EventHandler<PointsAccruedEventArgs> PointsAccrued = delegate { };
    public OrderService(InventoryService inv, IRepository<Order, string> r)
    {
        inventory = inv;
        repo = r;
    }
    public bool Submit(Order o)
    {
        foreach (var line in o.Lines)
        {
            if (!inventory.ReduceStock(line.product, line.Quantity))
                return false;
            o.ChangeStatus(OrderStatus.Confirmed);
            repo.Add(o);
            return true;
        }
        return false;
    }
    public void Pay(Order o)
    {
        if (o.Status != OrderStatus.Confirmed)
            throw new InvalidOrderStatusException($"Khong the thanh toan khi trang thai don hang la {o.Status}.");


        o.ChangeStatus(OrderStatus.Paid);

        if (o.Customer is MemberCustomer m)
        {
            int points = (int)Math.Floor(o.Total / 50_000m);
            m.Point += points;
            PointsAccrued(this, new PointsAccruedEventArgs(m.CustomerID, m.fullName, points, m.Point));
        }

        repo.Update(o);
    }
     public Order FindOrder(string orderNo)
    {
        if (!repo.TryGetById(orderNo, out var order))
        {
            throw new OrderNotFoundException($"Khong tim thay don hang voi ma {orderNo}.");
        }
        return order;
    }
}