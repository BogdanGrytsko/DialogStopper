using System;

namespace Trader.Index
{
    public class FormatIndicator
    {
        public string Date { get; set; }
        public decimal Value { get; set; }

        public static FormatIndicator Get5MinIndicator(Indicator x)
        {
            return new FormatIndicator
            {
                Date = x.Date.ToString("HH:mm"),
                Value = Math.Round(x.Value, 2)
            };
        }
    }
}