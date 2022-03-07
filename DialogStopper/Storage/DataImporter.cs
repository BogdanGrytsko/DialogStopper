using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace DialogStopper.Storage
{
    public class DataImporter<T, TMap> where TMap : ClassMap<T>
    {
        public List<T> LoadData(string path)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null
            };
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<TMap>();
            var records = csv.GetRecords<T>();
            return records.ToList();
        }
    }
}