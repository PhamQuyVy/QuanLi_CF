using QuanLi_CF.Domain;
using QuanLi_CF.Interface;
namespace QuanLi_CF.Strategies;
public class DefaultPriceRule : IPriceRule
{
    public decimal ComputeLineAmount(OrderLine line) => line.LineAmount;
}
