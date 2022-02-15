using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace Trader.MyFxBook
{
    public class MyFxBookDataImporter
    {
        public List<Candle> LoadData(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<MyFxBookCandleMap>();
                var records = csv.GetRecords<Candle>();
                return records.ToList();
            }
        }
    }
}