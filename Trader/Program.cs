using Trader.Data;
using Trader.Dividends;

namespace Trader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await StooqCandleDataMover.Move();
            //await DividendsDataMover.Move();
            var readModel = new DividendsReadModel(new TradingContext());
            var dividendsStrategy = new DividendsStrategy(readModel);
            await dividendsStrategy.RunCalendar();
            //await dividendsStrategy.RunPerSymbolPerYear();
        }
    }
}