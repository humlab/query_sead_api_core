using SeadQueryCore;
using SQT.Fixtures;
using SQT.Infrastructure;
using SQT.SQL.Matcher;
using Xunit;

namespace SQT.SqlCompilers
{
    [Collection("JsonSeededFacetContext")]
    public class DiscreteCategoryCountSqlCompilerTests : DisposableFacetContextContainer
    {
        public DiscreteCategoryCountSqlCompilerTests(JsonSeededFacetContextFixture fixture) : base(fixture)
        {
        }

        [Theory]
        [InlineData("sites:sites", "sites", "count")]
        [InlineData("country:sites", "sites", "count")]
        [InlineData("country@57:sites@3", "sites", "count")]
        public void Compile_VariousConfigs_ExpectedBehavior(string uri, string facetCode, string aggType)
        {
            // Arrange
            var mockQuerySetupFactory = new MockQuerySetupFactory(Registry);
            var querySetup = mockQuerySetupFactory.Scaffold(uri);
            var facet = Registry.Facets.GetByCode(facetCode);
            var countFacet = facet;

            // Act

            var discreteCategoryCountSqlCompiler = new DiscreteCategoryCountSqlCompiler();
            var sqlQuery = discreteCategoryCountSqlCompiler.Compile(querySetup, facet, countFacet, aggType);

            // Assert

            var matcher = new DiscreteCategoryCountSqlCompilerMatcher();

            var match = matcher.Match(sqlQuery.Squeeze());

            Assert.True(match.Success);

         }
    }
}
