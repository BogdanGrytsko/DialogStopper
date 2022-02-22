using System;

namespace Trader.Index
{
    //SMA = Simple Moving Average
    public class Indicator
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public decimal SMAValue { get; set; }
        
        public Indicator(decimal value, decimal smaValue, DateTime date)
        {
            Value = value;
            SMAValue = smaValue;
            Date = date;
        }
        
        public Indicator()
        {
        }
    }
}