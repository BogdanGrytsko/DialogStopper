using System.Diagnostics;
using DialogStopper.Storage;
using Newtonsoft.Json;

namespace DialogStopper
{
    class Program
    {
        private const string logFile = "Log.txt";

        static async Task Main(string[] args)
        {
            // await new MeditationGoogleSheetStorage().AddFromFile(logFile);
            // await new MeditationGoogleSheetStorage().UpdateStats(597);
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

                if (char.ToLower(c.KeyChar) is 'f' or 'j' or 'а' or 'о')
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

            var meditation = new Meditation(DateTime.UtcNow, points);
            await File.AppendAllTextAsync(logFile, JsonConvert.SerializeObject(meditation));
            await new MeditationGoogleSheetStorage().AddAsync(meditation);
            Console.WriteLine("Store completed");
        }
    }
}
