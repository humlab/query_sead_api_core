using Autofac.Features.Indexed;
using Moq;
using SeadQueryCore;
using SeadQueryCore.Model;
using SeadQueryCore.QueryBuilder;
using SeadQueryCore.Services.Result;
using SeadQueryTest.Infrastructure;
using SeadQueryTest.Mocks;
using System.Collections.Generic;
using Xunit;

namespace SeadQueryTest.Services.Result
{
    [Collection("JsonSeededFacetContext")]
    public class MapResultServiceTests : DisposableFacetContextContainer
    {
        public MapResultServiceTests(JsonSeededFacetContextFixture fixture) : base(fixture)
        {
        }

        private IQuerySetupCompiler MockQuerySetupCompiler()
        {
            IFacetsGraph facetsGraph = ScaffoldUtility.DefaultFacetsGraph(Registry);

            var mockPickCompiler = new Mock<IPickFilterCompiler>();
            mockPickCompiler
                .Setup(x => x.Compile(It.IsAny<Facet>(), It.IsAny<Facet>(), It.IsAny<FacetConfig2>()))
                .Returns("");

            var pickCompilers = new Mock<IPickFilterCompilerLocator>();
            pickCompilers.Setup(x => x.Locate(It.IsAny<EFacetType>())).Returns(mockPickCompiler.Object);

            IQuerySetupCompiler querySetupCompiler = new QuerySetupCompiler(facetsGraph, pickCompilers.Object, new JoinSqlCompiler());
            return querySetupCompiler;
        }

        //[Fact]
        //public void Load_StateUnderTest_ExpectedBehavior()
        //{
        //    using (var registry = FakeFacetsGetByCodeRepositoryFactory.Create()) {

        //        // Arrange
        //        var uri = "sites:sites";
        //        var facetsConfig = new MockFacetsConfigFactory(Registry.Facets).Create(uri);
        //        var resultKeys = new List<string>() { "site_level" };
        //        var resultConfig = ResultConfigFactory.Create("map", resultKeys);

        //        var mockCategoryCountServices = new Mock<IDiscreteCategoryCountService>();
        //        mockCategoryCountServices
        //            .Setup(x => x.Load(It.IsAny<string>(), It.IsAny<FacetsConfig2>(), It.IsAny<string>()))
        //            .Returns(
        //                new CategoryCountService.CategoryCountResult
        //                {
        //                    Data = new Dictionary<string, CategoryCountItem> {
        //                        { "A", new CategoryCountItem { Category = "A", Count = 10, Extent = new List<decimal>() } },
        //                        { "B", new CategoryCountItem { Category = "B", Count = 11, Extent = new List<decimal>() } },
        //                        { "C", new CategoryCountItem { Category = "C", Count = 12, Extent = new List<decimal>() } }
        //                    },
        //                    SqlQuery = ""
        //                }
        //            );

        //        var mockResultCompiler = new Mock<IResultCompiler>();

        //        mockResultCompiler
        //            .Setup(x => x.Compile(It.IsAny<FacetsConfig2>(), It.IsAny<ResultConfig>(), It.IsAny<string>()))
        //            .Returns(
        //                "SELECT DISTINCT tbl_sites.site_id AS id_column, tbl_sites.site_name AS name, coalesce(latitude_dd, 0.0) AS latitude_dd, coalesce(longitude_dd, 0) AS longitude_dd FROM tbl_sites WHERE 1 = 1 "
        //            );

        //        var service = new MapResultService(registry, mockResultCompiler.Object, mockCategoryCountServices.Object);

        //        // Act
        //        var resultSet = service.Load(facetsConfig, resultConfig);

        //        Assert.NotNull(resultSet);
        //    }
        //}

        private IResultQueryCompiler ConcreteResultCompiler(IRepositoryRegistry registry)
        {
            var resultSqlQueryCompilers = new MockIndex<string, IResultSqlQueryCompiler> {
                {  "map", new MapResultSqlQueryCompiler() }
            };
            IQuerySetupCompiler querySetupCompiler = MockQuerySetupCompiler();
            var resultQueryCompiler = new ResultQueryCompiler(registry, querySetupCompiler, resultSqlQueryCompilers);
            return resultQueryCompiler;
        }

        //private IDiscreteCategoryCountService ConcreteDiscreteCategoryCountService(IRepositoryRegistry registry)
        //{
        //    IFacetSetting facetSettings = new SettingFactory().DefaultFacetSettings();
        //    IQuerySetupCompiler querySetupCompiler = CreateQuerySetupCompiler(registry);
        //    IDiscreteCategoryCountSqlQueryCompiler categoryCountSqlCompiler = new DiscreteCategoryCountSqlQueryCompiler();
        //    return new DiscreteCategoryCountService(
        //        facetSettings,
        //        registry,
        //        querySetupCompiler,
        //        categoryCountSqlCompiler
        //    );
        //}
    }
}
