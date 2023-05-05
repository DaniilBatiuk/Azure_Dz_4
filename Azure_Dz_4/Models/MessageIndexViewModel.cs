using Azure_Dz_4.Models.DTOs;

namespace Azure_Dz_4.Models
{
    public class MessageIndexViewModel
    {
        public IEnumerable<PurchaseLotDTO> PurchaseLotDTOs { get; set; } = default!;

        public string? CurrencyName { get; set; }
    }
}
