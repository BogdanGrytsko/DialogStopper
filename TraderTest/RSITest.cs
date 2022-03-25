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
            var i1 = await t1;
            var i2 = await t2;
            var x1 = sw.Elapsed;

            sw.Restart();
            Task t3;
            Task t4;
            var i3 = await T1();
            var i4 = await T2();
            var x2 = sw.Elapsed;
        }

        private static async Task<int> T1()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return 1;
        }
        
        private static async Task<int> T2()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return 2;
        }
    }    
}

