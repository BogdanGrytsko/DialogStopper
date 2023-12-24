namespace Trader
{
    public class Candle
    {
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volume { get; set; }

        public bool IsUp => Change >= 0;
        public bool IsDown => Change < 0;
        public decimal Change => Close - Open;
        public decimal PercentChange => Change / Close;
        public decimal ClosePercentChange { get; set; }

        public override string ToString()
        {
            return $"{Date} {Change}";
        }
    }
}