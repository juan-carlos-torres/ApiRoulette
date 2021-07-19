using System;
using System.Collections.Generic;

namespace Roulette.DAL.DataAccessDTO
{
    public class RouletteDTO
    {
        public Guid ID { get; set; }
        public bool IsOpen { get; set; }
        public int? WinningNumber { get; set; }
        public List<BetDTO> BetList { get; set; }
    }
}
