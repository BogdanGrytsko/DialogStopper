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
    }    
}

