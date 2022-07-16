using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DialogStopper.Storage;

namespace DialogStopper
{
    class Program
    {
        private const string logFile = "Log.txt";

        static async Task Main(string[] args)
        {
            // await new MeditationGoogleSheetStorage().AddFromFile(logFile);
            // await new MeditationGoogleSheetStorage().UpdateStats();
            // var x = await new MeditationGoogleSheetStorage().Get();
            // await new MeditationGoogleSheetStorage().Delete(189);
            var points = new List<(long, PointType)>();
            var sw = new Stopwatch();
            var started = false;
            Console.WriteLine("Program Started. Press Space to start logging");
            while (true)
            {
                var c = Console.ReadKey();
                var time = sw.ElapsedMilliseconds / 1000;
                if (c.Key == ConsoleKey.Spacebar)
                {
                    if (!started)
                    {
                        sw.Start();
                        Console.WriteLine("Started");
                        started = true;
                    }
                    else
                    {
                        points.Add((time, PointType.SingleThought));    
                    }
                }

                if (c.Key is ConsoleKey.F or ConsoleKey.J)
                {
                    points.Add((time, PointType.ContinuousSegment));
                }

                if (c.Key == ConsoleKey.Enter)
                {
                    points.Add((time, PointType.SingleThought));
                    sw.Stop();
                    Console.WriteLine("Finished");
                    break;
                }
            }
            
            await File.AppendAllTextAsync(logFile, $"{DateTime.UtcNow}: {points.Count}. {string.Join(",", points)}{Environment.NewLine}");
            await new MeditationGoogleSheetStorage().Add(new Meditation(DateTime.UtcNow, points));
            Console.WriteLine("Store completed");
        }
    }
}
