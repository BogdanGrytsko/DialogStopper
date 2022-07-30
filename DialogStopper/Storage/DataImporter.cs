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
        public List<T> LoadData(string path, string delimiter = null)
        {
            using var reader = new StreamReader(path);
            return LoadData(reader, delimiter);
        }
        
        public List<T> LoadData(StreamReader reader, string delimiter = null)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
            };
            if (delimiter != null)
                config.Delimiter = delimiter;
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<TMap>();
            var records = csv.GetRecords<T>();
            return records.ToList();
        }

        public List<T> LoadData(byte[] file, string delimiter = null)
        {
            using var ms = new MemoryStream(file);
            using var reader = new StreamReader(ms, true);
            return LoadData(reader, delimiter);
        }
    }
}