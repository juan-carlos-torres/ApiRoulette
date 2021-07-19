using System;

namespace Roulette.DAL.DataAccessDTO
{
    public class BetDTO
    {
        public Guid UserID { get; set; }
        public decimal Amount { get; set; }
        public bool BetByNumber { get; set; }
        public int? Number { get; set; }
        public string Color { get; set; }
    }
}
