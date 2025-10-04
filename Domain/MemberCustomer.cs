namespace QuanLi_CF.Domain;

public class MemberCustomer : Customer
{
    public string MemberCode { get; set; } = "";
    public MemberTier Tier { get; set; } = MemberTier.Standard;
    public int Point { get; set; }
    public override decimal GetDiscountPercent(Order order)
    {
        return Tier switch

        {
            MemberTier.Standard => 0.02m,
            MemberTier.Bronze => 0.04m,
            MemberTier.Silver => 0.06m,
            MemberTier.Gold => 0.08m,
            MemberTier.Platinum => 0.1m,
            _ => 0m
        };
    }
}