namespace Trader.Index
{
    //SMA = Simple Moving Average
    public class Indicator
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        
        public Indicator(decimal value, DateTime date)
        {
            Value = value;
            Date = date;
        }
        
        public Indicator()
        {
        }
    }
}