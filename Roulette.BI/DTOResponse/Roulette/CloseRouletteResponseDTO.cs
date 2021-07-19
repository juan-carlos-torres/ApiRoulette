using System;

namespace Roulette.BI.DTOResponse.Roulette
{
    public class CloseRouletteResponseDTO
    {
        public Guid UserID { get; set; }
        public decimal Amount { get; set; }
        public decimal EarnedValue { get; set; }
    }
}
