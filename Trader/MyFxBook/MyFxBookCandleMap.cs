using CsvHelper.Configuration;

namespace Trader.MyFxBook
{
    public class MyFxBookCandleMap : ClassMap<Candle>
    {
        public MyFxBookCandleMap()
        {
            Map(m => m.Date).Name("Date");
            Map(m => m.Open);
            Map(m => m.High);
            Map(m => m.Low);
            Map(m => m.Close);
        }
    }
}