using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace TraderTest.Interview
{
    public class TaskAsyncTest
    {
        [Fact]
        public async Task TaskTest()
        {
            var sw = Stopwatch.StartNew();
            var t1 = T1();
            var t2 = T2();
            await t1;
            await t2;
            var x1 = sw.Elapsed;

            sw.Restart();
            await T1();
            await T2();
            var x2 = sw.Elapsed;
        }

        private static async Task T1()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        
        private static async Task T2()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}