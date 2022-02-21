using System;
using System.Collections.Generic;

namespace Trader.Index
{
    /// <summary>
    /// Relative Strength Index
    /// </summary>
    public class RSI : IIndex
    {
        private int lookBackBarsCount;

        public RSI(int lookBackBarsCount = 14)
        {
            this.lookBackBarsCount = lookBackBarsCount;
        }
        
        public decimal Calculate(List<Candle> candles)
        {
            if (candles.Count < lookBackBarsCount)
                throw new Exception("Not Enough data. TODO");

            
            throw new System.NotImplementedException();
        }

        public List<Indicator> CalculateMany(List<Candle> candles)
        {
            //todo : test. Also should produce with and without SMA
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
                        gain -= addCandle.PercentChange;
                    else
                        loss -= Math.Abs(addCandle.PercentChange);
                }

                if (i < lookBackBarsCount - 1)
                    continue;
                
                if (i == lookBackBarsCount - 1)
                {
                    var rs = gain / loss;
                    indicators.Add(new Indicator(GetRsi(rs), addCandle.Date));
                }
                else
                {
                    var movingGain = previousGain * (lookBackBarsCount - 1) + gain;
                    var movingLoss = previousLoss * (lookBackBarsCount - 1) + loss;
                    var rs = movingGain / movingLoss;
                    indicators.Add(new Indicator(GetRsi(rs), addCandle.Date));
                }
                
                previousGain = gain;
                previousLoss = loss;
            }
            return indicators;
        }

        private static decimal GetRsi(decimal rs)
        {
            return 100 - Math.Floor(100 / (1 + rs));
        }
    }
}