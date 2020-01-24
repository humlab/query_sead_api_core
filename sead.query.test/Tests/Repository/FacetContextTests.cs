using SeadQueryInfra.DataAccessProvider;
using Moq;
using SeadQueryCore;
using SeadQueryInfra;
using SeadQueryTest.Infrastructure;
using SeadQueryTest.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace SeadQueryTest.Repository
{
    public class FacetContextTests: IDisposable
    {
        private MockRepository mockRepository;

        private Mock<ISetting> mockQueryBuilderSetting;

        public FacetContextTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockQueryBuilderSetting = this.mockRepository.Create<ISetting>();
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private FacetContext CreateFacetContext()
        {
            var options = new DbContextOptionsBuilder<FacetContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            var facetContext = new FacetContext(options);
            return facetContext;
        }

        [Fact(Skip ="Not implemented")]
        public void SaveChanges_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            using (var facetContext = CreateFacetContext()) {

                // Act
                var result = facetContext.SaveChanges();

                // Assert
                Assert.True(true);
            }
        }

        //[Fact]
        //public void Context_Should_Have_Values_For_All_Entity_Types()
        //{
        //    using (var context = FakeFacetContextFactory.JsonSeededFacetContext()) {

        //        foreach (Type type in ScaffoldUtility.GetModelTypes()) {
        //            var g = GetGenericMethodForType<FacetContext>("Set", type);
        //            var entities = (IEnumerable<object>)(g.Invoke(context, Array.Empty<object>()));
        //            Assert.True(entities.ToList().Count > 0);
        //        }
        //    }
        //}

        //private object GetGenericMethodForType<T>(string v, Type type)
        //{
        //    throw new NotImplementedException();
        //}

        [Fact]
        public void ShouldBeAbleToFetchFacetAndReferenceObjects()
        {
            var repository = JsonSeededRepositoryRegistryFactory.Create().Facets;

            Facet facet = repository.Get(25);

            Dictionary<string, object> expectedProperties = new Dictionary<string, object>() {
                { "FacetId", 25 },
                { "FacetCode", "species" },
                { "DisplayTitle", "Taxa" },
                { "FacetGroupId", 6 },
                { "FacetTypeId", EFacetType.Discrete },
                { "CategoryIdExpr", "tbl_taxa_tree_master.taxon_id" },
                { "CategoryNameExpr", "concat_ws(' ', tbl_taxa_tree_genera.genus_name, tbl_taxa_tree_master.species, tbl_taxa_tree_authors.author_name)" },
                { "SortExpr", "tbl_taxa_tree_genera.genus_name||' '||tbl_taxa_tree_master.species" },
                { "IsApplicable", true },
                { "IsDefault", false },
                { "AggregateType", "sum" },
                { "AggregateTitle", "sum of Abundance" },
                { "AggregateFacetId", 32 }
            };

            Asserter.EqualByProperty(expectedProperties, facet);

            Assert.NotNull(facet.FacetGroup);
            Assert.NotNull(facet.TargetTable);
            Assert.NotNull(facet.FacetType);
            Assert.NotNull(facet.Tables);

            Assert.True(facet.Tables.Count > 0);

        }

        [Fact]
        public void CanGetAliasFacets()
        {
            var repository = JsonSeededRepositoryRegistryFactory.Create().Facets;

            Facet facet = repository.Get(21);

            Assert.Equal("country", facet.FacetCode);
            Assert.Equal("Country", facet.DisplayTitle);
            Assert.NotNull(facet.FacetGroup);
            Assert.NotNull(facet.TargetTable);
            Assert.NotNull(facet.FacetType);
            Assert.True(facet.Tables.Count > 0);

            List<Facet> aliasFacets = repository.FindThoseWithAlias().ToList();
            Assert.Single(aliasFacets);
            Assert.Same(facet, aliasFacets[0]);

        }
    }
}
