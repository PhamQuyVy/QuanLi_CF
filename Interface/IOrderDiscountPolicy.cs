using QuanLi_CF.Domain;
namespace QuanLi_CF.Interface;
public interface IOrderDiscountPolicy { decimal ComputeOrderLevelDiscount(Order order); }
