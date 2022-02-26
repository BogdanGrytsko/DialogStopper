using System;
using System.Collections.Generic;

namespace Trader.Index
{
    /// <summary>
    /// Relative Strength Index
    /// </summary>
    public class RSI
    {
        private readonly int length;

        public RSI(int length = 14)
        {
            this.length = length;
        }
        
        public List<Indicator> CalculateMany(List<Candle> candles)
        {
            var indicators = new List<Indicator>();
            decimal gain = 0, loss = 0, previousGain = 0, previousLoss = 0;
            for (int i = 0; i < candles.Count; i++)
            {
                var currCandle = candles[i];
                var percentChange = i == 0 ? currCandle.PercentChange : GetPercentChange(currCandle, candles[i - 1]);
                currCandle.ClosePercentChange = percentChange;
                gain += Math.Max(percentChange, 0);
                loss += Math.Max(percentChange * -1, 0);
                
                var subCandleIndex = i - length;
                if (subCandleIndex >= 0)
                {
                    var subCandle = candles[subCandleIndex];
                    gain -= Math.Max(subCandle.ClosePercentChange, 0);
                    loss -= Math.Max(subCandle.ClosePercentChange * -1, 0);
                }

                if (i < length - 1)
                    continue;
                
                if (i == length - 1)
                {
                    var rs = gain / loss;
                    var rsi = GetRsi(rs);
                    indicators.Add(new Indicator(rsi, currCandle.Date));
                }
                else
                {
                    var movingGain = previousGain * (length - 1) + gain;
                    var movingLoss = previousLoss * (length - 1) + loss;
                    var movingRs = movingGain / movingLoss;
                    indicators.Add(new Indicator(GetRsi(movingRs), currCandle.Date));
                }
                
                previousGain = gain;
                previousLoss = loss;
            }
            return indicators;
        }

        private static decimal GetPercentChange(Candle curr, Candle prev)
        {
            return (curr.Close - prev.Close) / curr.Close;
        }

        public static decimal GetRsi(decimal rs)
        {
            if (rs < 0)
                throw new ArgumentException("Can't be negative", nameof(rs));
            return rs * 100 / (1 + rs);
        }
    }
}