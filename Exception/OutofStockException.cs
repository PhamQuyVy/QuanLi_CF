namespace QuanLi_CF.Exceptions;
public class OutofStockException : Exception
{
    public OutofStockException(string message) : base(message) { }
}