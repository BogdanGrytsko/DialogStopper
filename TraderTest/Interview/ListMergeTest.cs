using System.Collections.Generic;
using Xunit;

namespace TraderTest.Interview
{
    public class ListMergeTest
    {
        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [Fact]
        public void ListTest()
        {
            var list1 = new List<Person>
            {
                new() { Id = 1, Name = "Bogdan" },
                new() { Id = 2, Name = "Roman" }
            };
            var list2 = new List<Person>
            {
                new() { Id = 2, Age = 20 },
                new() { Id = 3, Age = 30}
            };
            //Code goes here
            var expectedList = new List<Person>
            {
                new() { Id = 1, Name = "Bogdan" },
                new() { Id = 2, Name = "Roman", Age = 20},
                new() { Id = 3, Age = 30}
            };
        }

        [Fact]
        public void DictionaryTest()
        {
            var dict = new Dictionary<Person, int>();
            dict[new Person { Name = "AA" }] = 1;
            var x = dict[new Person { Name = "AA" }];
        }
    }
}