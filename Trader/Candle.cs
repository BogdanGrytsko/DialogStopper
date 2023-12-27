using System.ComponentModel.DataAnnotations;

namespace Trader
{
    public class Candle
    {
        public int Id { get; set; }
        [Required, StringLength(4)]
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volume { get; set; }

        public decimal Change => Close - Open;
        public decimal PercentChange => Change / Close;
        public decimal ClosePercentChange { get; set; }

        public override string ToString()
        {
            return $"{Date} {Change}";
        }
    }
}