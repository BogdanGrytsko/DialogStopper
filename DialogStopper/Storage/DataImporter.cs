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
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TMap>();
            var records = csv.GetRecords<T>();
            return records.ToList();
        }
    }
}