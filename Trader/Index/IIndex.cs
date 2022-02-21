using System.Collections.Generic;

namespace Trader.Index
{
    public interface IIndex
    {
        //all inputs must be sorted - old dates first
        decimal Calculate(List<Candle> candles);
    }
}