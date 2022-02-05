using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DialogStopper
{
    class Program
    {
        private const string logFile = "Log.txt";

        static async Task Main(string[] args)
        {
            await new GoogleSheetStorage().SyncAllHistory();
            var points = new List<long>();
            var sw = new Stopwatch();
            while (true)
            {
                var c = Console.ReadKey();
                if (c.Key == ConsoleKey.C)
                {
                    sw.Start();
                    Console.WriteLine("Started");
                }

                if (c.Key == ConsoleKey.Spacebar)
                {
                    points.Add(sw.ElapsedMilliseconds / 1000);
                }

                if (c.Key == ConsoleKey.Enter)
                {
                    points.Add(sw.ElapsedMilliseconds / 1000);
                    sw.Stop();
                    Console.WriteLine("Finished");
                    break;
                }
            }
            
            File.AppendAllText(logFile, $"{DateTime.UtcNow}: {points.Count}. {string.Join(",", points)}{Environment.NewLine}");
            LogTransformer.ToGoogleSheets(logFile);
        }
    }
}
