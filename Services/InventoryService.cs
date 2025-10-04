using QuanLi_CF.Domain;
using QuanLi_CF.Exceptions;
using QuanLi_CF.Interface;
namespace QuanLi_CF.Services;

public class InventoryService
{
    private readonly IRepository<DrinkProduct, string> repo;
    private readonly int threshold;
    public event EventHandler<DrinkProduct> LowStock = delegate { };
    public InventoryService(IRepository<DrinkProduct, string> r, int t)
    {
        repo = r;
        threshold = t;
    }
    public bool ReduceStock(DrinkProduct p, int qty)
    {

      if (p.StockQty < qty)
      throw new OutofStockException($"Khong du hang cho san pham {p.Name}. Quy khach yeu cau {qty}, con lai {p.StockQty}.");

        p.StockQty -= qty;
        repo.Update(p);
        if (p.StockQty < threshold)
        {
            LowStock(this, p);
        }
        return true;
    }

}