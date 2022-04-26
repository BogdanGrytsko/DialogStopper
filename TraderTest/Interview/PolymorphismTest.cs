using System;
using System.Collections.Generic;
using Xunit;

namespace TraderTest.Interview
{
    public class PolymorphismTest
    {
        public class Animal
        {
            public void A() { Console.WriteLine("Animal.A"); }
            public virtual void B() { Console.WriteLine("Animal.B"); }
        }
        
        public class Dog : Animal
        {
            public new void A() { Console.WriteLine("Dog.A"); }
            public override void B() { Console.WriteLine("Dog.B"); }
        }
        
        [Fact]
        public void AnimalDogTest()
        {
            var d = new Dog();
            var a = (Animal)d;
            a.A();
            a.B();
        }
        
        private static void A(List<string> a)
        {
        }
        
        private static void B(ref List<string> a)
        {
        }
    }
}