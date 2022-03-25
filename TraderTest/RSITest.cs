using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Trader.Index;
using Xunit;

namespace TraderTest
{
    public class RSITest
    {
        [Fact]
        public void Highest()
        {
            var high = RSI.GetRsi(100000);
            high.Should().BeGreaterThan(99.9M);
        }
        
        [Fact]
        public void LowTest()
        {
            var low = RSI.GetRsi(0);
            low.Should().Be(0);
        }

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

