using System;
using System.Collections.Generic;

namespace Trader.Index
{
    /// <summary>
    /// Relative Strength Index
    /// </summary>
    public class RSI
    {
        private readonly int lookBackBarsCount;

        public RSI(int lookBackBarsCount = 14)
        {
            this.lookBackBarsCount = lookBackBarsCount;
        }
        
        public List<Indicator> CalculateMany(List<Candle> candles)
        {
            var indicators = new List<Indicator>();
            decimal gain = 0, loss = 0, previousGain = 0, previousLoss = 0;
            for (int i = 0; i < candles.Count; i++)
            {
                var addCandle = candles[i];
                if (addCandle.IsUp)
                    gain += addCandle.PercentChange;
                else
                    loss += Math.Abs(addCandle.PercentChange);
                
                var subCandleIndex = i - lookBackBarsCount;
                if (subCandleIndex >= 0)
                {
                    var subCandle = candles[subCandleIndex];
                    if (subCandle.IsUp)
                        gain -= subCandle.PercentChange;
                    else
                        loss -= Math.Abs(subCandle.PercentChange);
                }

                if (i < lookBackBarsCount - 1)
                    continue;
                
                if (i == lookBackBarsCount - 1)
                {
                    var rs = gain / loss;
                    var rsi = GetRsi(rs);
                    indicators.Add(new Indicator(rsi, rsi, addCandle.Date));
                }
                else
                {
                    var movingGain = previousGain * (lookBackBarsCount - 1) + gain;
                    var movingLoss = previousLoss * (lookBackBarsCount - 1) + loss;
                    var movingRs = movingGain / movingLoss;
                    var rsi = GetRsi(gain / loss);
                    indicators.Add(new Indicator(rsi ,GetRsi(movingRs), addCandle.Date));
                }
                
                previousGain = gain;
                previousLoss = loss;
            }
            return indicators;
        }

        public static decimal GetRsi(decimal rs)
        {
            if (rs < 0)
                throw new ArgumentException("Can't be negative", nameof(rs));
            return 100 - (100 / (1 + rs));
        }
    }
}