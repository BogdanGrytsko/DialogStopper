using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DialogStopper.Storage;

namespace PsychologyCourseWork
{
    public class DataCreator
    {
        public async Task Create()
        {
            LifeEventsMethod.Load();
            var sep = '\t';
            var path = @"C:\temp\Опитувальник для магістерської дипломної роботи (Відповіді) (1) - Відповіді форми (1).tsv";
            var lines = await File.ReadAllLinesAsync(path);
            var headers = lines[0].Split(sep);
            var persons = new List<Person>();
            foreach (var line in lines.Skip(1))
            {
                var values = line.Split(sep);
                var person = ParseLine(values, headers);
                persons.Add(person);
            }
            DataExporter.Export(persons, @"C:\temp\Опитувальник Результати.csv");
        }

        private static Person ParseLine(string[] values, string[] headers)
        {
            var person = new Person
            {
                TimeStamp = values[0],
                Age = values[1],
                Sex = values[2].Contains("Ж", StringComparison.OrdinalIgnoreCase) ? "Female" : "Male",
                Qualification = values[3],
                RogersDiamondMethod = new SocialPsychologicalRogersDiamondMethod(values, headers),
                NeuroticAnxietyMethod = new NeuroticAnxietyMethod(values),
                LifeEventsMethod = new LifeEventsMethod(values)
            };
            return person;
        }
    }    
}
