using DataAnnotationsExtensions;
using System;

namespace Roulette.BI.DTORequest.Roulette
{
    public class AddRouletteBetRequestDTO
    {
        public Guid RouletteID { get; set; }

        [Max(10000, ErrorMessage = "Máximo puede apostar {1} dólares")]
        public decimal Amount { get; set; }
        public bool BetByNumber { get; set; }

        [Min(0, ErrorMessage = "El valor mínimo es {1}")]
        [Max(36, ErrorMessage = "El valor máximo es {1}")]
        public int? Number { get; set; }
        public string Color { get; set; }

    }
}
