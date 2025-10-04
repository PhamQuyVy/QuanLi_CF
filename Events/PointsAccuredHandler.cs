namespace QuanLi_CF.Events;

public class PointsAccruedEventArgs : EventArgs
{
    public string CustomerId { get; }
    public string CustomerName { get; }
    public int PointsAdded { get; }
    public int TotalPoints { get; }

    public PointsAccruedEventArgs(string customerId, string customerName, int pointsAdded, int totalPoints)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        PointsAdded = pointsAdded;
        TotalPoints = totalPoints;
    }
}
