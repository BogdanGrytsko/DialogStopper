namespace Trader.Index
{
    /// <summary>
    /// Simple Moving Average. Users can offset as the want
    /// </summary>
    public class SMA
    {
        private readonly int length;

        public SMA(int length)
        {
            this.length = length;
        }

        public List<Indicator> CalculateMany(List<Candle> candles)
        {
            var indicators = new List<Indicator>();
            var sum = 0m;
            for (int i = 0; i < candles.Count; i++)
            {
                var currCandle = candles[i];
                sum += currCandle.Close;
                
                var subCandleIndex = i - length;
                if (subCandleIndex >= 0)
                {
                    var subCandle = candles[subCandleIndex];
                    sum -= subCandle.Close;
                }

                if (i < length - 1)
                    continue;

                var avg = sum / length;
                indicators.Add(new Indicator(avg, currCandle.Date));
            }
            
            return indicators;
        }
    }
}