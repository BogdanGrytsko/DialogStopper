using Trader.Dividends;

namespace Trader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dividendsStrategy = new DividendsStrategy();
            await dividendsStrategy.Run();
        }

        //static async Task Main(string[] args)
        //{
        //    //https://youtu.be/LfysewNHmDE
        //    var data = new DataImporter<Candle, MyFxBookCandleMap>().LoadData("MyFxBook\\EURUSD_5min_FEB_14-15_2022.csv");
        //    data = data.Where(x => x.Date < new DateTime(2022, 2, 15)).Reverse().ToList();
        //    var upPoints = GetUpDownPoints(data).ToList();
        //    var downPoints = GetDownUpPoints(data).ToList();
        //    //how to get local ups?
        //    //it's NEXT HIGHEST upPOint. In a downTrend. But also in uptrend?!
        //    //NEXT HIGHEST upPOint smaller than previous?
        //    //I'm trying to determine down\uptrend??
        //    //it's for sure is related to the time span selected. Can give either up down or side
        //    //but can we find subtrends in a big timesapn?
        //    //we have to put precision - e.g number of subtrends to find
        //    //list of maximums of updowns, then order by timestamp
        //    //how many maximums? 

        //    var rsi = new RSI().CalculateMany(data);
        //    var rsiStorage = new GoogleSheetStorage<FormatIndicator>("1cJzzIJPyMIfoMkwTQVIsjG21zqgPg4bcaDcfFeEK8lY") {SheetName = "RSI"};
        //    await rsiStorage.Delete(1, rsi.Count + 1);
        //    await rsiStorage.Add(rsi.Select(FormatIndicator.Get5MinIndicator).ToList(), true);
            
        //    var sma = new SMA(10).CalculateMany(data);
        //    var smaStorage = new GoogleSheetStorage<FormatIndicator>("1cJzzIJPyMIfoMkwTQVIsjG21zqgPg4bcaDcfFeEK8lY") {SheetName = "SMA10"};
        //    await smaStorage.Delete(1, sma.Count + 1);
        //    await smaStorage.Add(sma.Select(FormatIndicator.Get5MinIndicator).ToList(), true);
        //    Console.WriteLine("Finished");
        //}

        //private static List<Candle> GetUpDownPoints(List<Candle> data)
        //{
        //    if (data.Count <= 1)
        //        return new List<Candle>();
        //    var result = new List<Candle>();
        //    for (int i = 1; i < data.Count; i++)
        //    {
        //        var thisCandle = data[i];
        //        var prevCandle = data[i - 1];
        //        // if (thisCandle.IsDown && prevCandle.IsUp)
        //        //     yield return thisCandle;
        //    }

        //    return result;
        //}
        
        //private static IEnumerable<Candle> GetDownUpPoints(List<Candle> data)
        //{
        //    if (data.Count <= 1)
        //        yield break;
        //    for (int i = 1; i < data.Count; i++)
        //    {
        //        var thisCandle = data[i];
        //        var prevCandle = data[i - 1];
        //        if (thisCandle.IsUp && prevCandle.IsDown)
        //            yield return thisCandle;
        //    }
        //}
    }
}