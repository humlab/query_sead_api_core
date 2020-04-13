using Autofac.Features.Indexed;
using Moq;
using SeadQueryCore;
using SeadQueryCore.QueryBuilder;
using SeadQueryTest.Infrastructure;
using System;
using Xunit;

namespace SeadQueryTest.Services
{
    [Collection("JsonSeededFacetContext")]
    public class RangeCategoryBoundsServiceTests : DisposableFacetContextContainer
    {
        public RangeCategoryBoundsServiceTests(JsonSeededFacetContextFixture fixture) : base(fixture)
        {
        }
    }
}
