using System.Threading.Tasks;
using Trader.MyFxBook;

namespace Trader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var data = new MyFxBookDataImporter().LoadData("MyFxBook\\EURUSD_5min_FEB_14-15_2022.csv");
        }
    }
}