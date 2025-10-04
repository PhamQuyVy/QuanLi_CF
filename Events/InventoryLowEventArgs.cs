using QuanLi_CF.Domain;
namespace QuanLi_CF.Events;

public delegate void InventoryLowEventHandler(object sender, InventoryLowEventArgs e);
public class InventoryLowEventArgs : EventArgs
{
    public DrinkProduct Product { get; }
    public InventoryLowEventArgs(DrinkProduct p) { Product = p; }
}
