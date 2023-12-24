using CsvHelper.Configuration;

namespace Trader.Data;

public class MarketWatchCandleMap : ClassMap<Candle>
{
    public MarketWatchCandleMap()
    {
        Map(m => m.Date).Name("Date");
        Map(m => m.Open);
        Map(m => m.High);
        Map(m => m.Low);
        Map(m => m.Close);
        Map(m => m.Volume);
    }
}