using CsvHelper.Configuration;

namespace Trader.Data;

public class StooqCandleMap : ClassMap<Candle>
{
    public StooqCandleMap()
    {
        Map(m => m.Symbol).Name("<TICKER>");
        Map(m => m.Date).Name("<DATE>");
        Map(m => m.Open).Name("<OPEN>");
        Map(m => m.High).Name("<HIGH>");
        Map(m => m.Low).Name("<LOW>");
        Map(m => m.Close).Name("<CLOSE>");
        Map(m => m.Volume).Name("<VOL>");
    }
}