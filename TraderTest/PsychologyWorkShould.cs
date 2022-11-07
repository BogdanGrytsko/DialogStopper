using System.Threading.Tasks;
using PsychologyCourseWork;
using Xunit;

namespace TraderTest
{
    public class PsychologyWorkShould
    {
        [Fact]
        public async Task CreateData()
        {
            var creator = new DataCreator();
            await creator.Create();
        }
    }
}