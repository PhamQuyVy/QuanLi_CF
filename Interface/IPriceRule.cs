using QuanLi_CF.Domain;
namespace QuanLi_CF.Interface;
public interface IPriceRule { decimal ComputeLineAmount(OrderLine line); }
