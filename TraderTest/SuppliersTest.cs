using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace TraderTest
{
    public class SuppliersTest
    {
        [Fact]
        public void Test()
        {
            var ecoDbPublished = File.ReadAllLines(@"C:\temp\Ecovadis\SuppliersBUG\SuppliersWithPublishedMetrics.csv");
            var elasticPublished = File.ReadAllLines(@"C:\temp\Ecovadis\SuppliersBUG\elasticsearchexport.csv");

            var ecoDbSuppliers = GetIds(ecoDbPublished);
            var elasticSuppliers = GetIds(elasticPublished);
            var result = elasticSuppliers.Except(ecoDbSuppliers).ToList();
            File.WriteAllLines(@"C:\temp\Ecovadis\SuppliersBUG\SuppliersInElasticButNotInEcoDb.csv", result.Select(x => x.ToString()));
        }

        private HashSet<int> GetIds(string[] ecoDbPublished)
        {
            return ecoDbPublished.Skip(1).Select(int.Parse).ToHashSet();
        }
    }
}