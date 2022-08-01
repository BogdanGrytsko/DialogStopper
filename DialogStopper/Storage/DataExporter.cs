using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace DialogStopper.Storage
{
    public class DataExporter
    {
        public static void Export<T>(List<T> data, string path, string delimiter = null)
        {
            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
                { Delimiter = delimiter ?? ";", Encoding = Encoding.UTF8 };
            using var writer = new StreamWriter(path);
            using var csv = new CsvWriter(writer, config);
            csv.WriteRecords(data);
        }
        
        public static void Export<T, TMap>(List<T> data, string path, string delimiter = null) where TMap : ClassMap
        {
            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
                { Delimiter = delimiter ?? ";", Encoding = Encoding.UTF8 };
            using var writer = new StreamWriter(path);
            using var csv = new CsvWriter(writer, config);
            csv.Context.RegisterClassMap<TMap>();
            csv.WriteRecords(data);
        }
    }
}