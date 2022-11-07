using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PsychologyCourseWork
{
    public class DataCreator
    {
        public async Task Create()
        {
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
            
            //we have Person info and then answers, 3-4 blocks of questions
            //result should be Person Info and then MAPPED answers
            //after that Person Info + calculated Answers
        }

        private static Person ParseLine(string[] values, string[] headers)
        {
            var person = new Person
            {
                TimeStamp = values[0],
                Age = values[1],
                Sex = values[2],
                Qualification = values[3],
                RogersDiamondMethod = new SocialPsychologicalRogersDiamondMethod(values, headers)
            };
            return person;
        }
    }    
}
