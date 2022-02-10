using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
            var points = new List<long>();
            var sw = new Stopwatch();
            Console.WriteLine("Program Started. Press C to start logging");
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

            await new MeditationGoogleSheetStorage().Add(new Meditation(DateTime.UtcNow, points));
        }
    }
}
