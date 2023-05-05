namespace Azure_Dz_4.Models.DTOs
{
    public class PurchaseLotDTO
    {
        public string CurrencyName { get; set; } = default!;
        public decimal Amount { get; set; } = default!;
        public string SellerName { get; set; } = default!;
    }
}
