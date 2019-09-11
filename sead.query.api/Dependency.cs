﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataAccessPostgreSqlProvider;
using Microsoft.Extensions.DependencyInjection;
using SeadQueryCore;
using SeadQueryCore.QueryBuilder;
using SeadQueryInfra;
using System;

namespace SeadQueryAPI
{

    //public interface IControllerServiceAggregate
    //{
    //    IQueryBuilderSetting Setting { get; set; }
    //    ICacheContainer QueryCache { get; set; }
    //    IRepositoryRegistry UnitOfWork { get; set; }
    //}

    public class DependencyService
    {

        public virtual ISeadQueryCache GetCache(StoreSetting settings)
        {
            try {
                if (settings?.UseRedisCache == true)
                    return new RedisCacheProvider();
            } catch (InvalidOperationException) {
                Console.WriteLine("Failed to connect to Redis!");
            }
            Console.WriteLine("Warning: Using in memory cache provider!");
            return new SimpleMemoryCacheProvider();
        }

        public virtual IContainer Register(IServiceCollection services, IQueryBuilderSetting options)
        {
            var builder = new Autofac.ContainerBuilder();

            // http://docs.autofac.org/en/latest/register/registration.html

            builder.RegisterInstance<IQueryBuilderSetting>(options).SingleInstance().ExternallyOwned();

            // builder.Register(c => GetCacheManager(options?.Store)).SingleInstance().ExternallyOwned();
            // builder.RegisterAggregateService<ICacheContainer>();
            builder.Register(z => GetCache(options?.Store)).SingleInstance().ExternallyOwned();

            // builder.RegisterType<DomainModelDbContext>().SingleInstance().InstancePerLifetimeScope();

            builder.RegisterType<FacetContext>().As<IFacetContext>().SingleInstance().InstancePerLifetimeScope();
            builder.RegisterType<RepositoryRegistry>().As<IRepositoryRegistry>().InstancePerLifetimeScope();

            builder.RegisterType<FacetGraphFactory>().As<IFacetGraphFactory>().InstancePerLifetimeScope();
            builder.Register<IFacetsGraph>(c => c.Resolve<IFacetGraphFactory>().Build());

            builder.RegisterType<QuerySetupBuilder>().As<IQuerySetupBuilder>();
            builder.RegisterType<DeleteBogusPickService>().As<IDeleteBogusPickService>();

            builder.RegisterType<RangeCategoryBoundsService>().As<ICategoryBoundsService>();

            builder.RegisterType<UndefinedFacetPickFilterCompiler>().Keyed<IPickFilterCompiler>(0);
            builder.RegisterType<DiscreteFacetPickFilterCompiler>().Keyed<IPickFilterCompiler>(1);
            builder.RegisterType<RangeFacetPickFilterCompiler>().Keyed<IPickFilterCompiler>(2);
            builder.RegisterType<GeoFacetPickFilterCompiler>().Keyed<IPickFilterCompiler>(3);

            #region __Count Services__
            builder.RegisterType<RangeCategoryCountService>().Keyed<ICategoryCountService>(EFacetType.Range);
            builder.RegisterType<DiscreteCategoryCountService>().Keyed<ICategoryCountService>(EFacetType.Discrete);

            //builder.RegisterAggregateService<ICategoryCountServiceAggregate>();
            //builder.RegisterType<RangeCategoryCountService>();
            //builder.RegisterType<DiscreteCategoryCountService>();
            #endregion

            builder.RegisterType<ValidPicksSqlQueryCompiler>().As<IValidPicksSqlQueryCompiler>();
            builder.RegisterType<EdgeSqlCompiler>().As<IEdgeSqlCompiler>();
            builder.RegisterType<DiscreteContentSqlQueryBuilder>().As<IDiscreteContentSqlQueryCompiler>();
            builder.RegisterType<DiscreteCategoryCountSqlQueryCompiler>().As<IDiscreteCategoryCountSqlQueryCompiler>();
            builder.RegisterType<RangeCategoryCountSqlQueryCompiler>().As<IRangeCategoryCountSqlQueryCompiler>();
            builder.RegisterType<RangeIntervalSqlQueryCompiler>().As<IRangeIntervalSqlQueryCompiler>();
            builder.RegisterType<RangeOuterBoundSqlCompiler>().As<IRangeOuterBoundSqlCompiler>();
            
            builder.RegisterType<RangeFacetContentService>().Keyed<IFacetContentService>(EFacetType.Range);
            builder.RegisterType<DiscreteFacetContentService>().Keyed<IFacetContentService>(EFacetType.Discrete);

            builder.RegisterType<ResultCompiler>().As<IResultCompiler>();

            //builder.RegisterAggregateService<IControllerServiceAggregate>();

            builder.RegisterType<RangeCategoryBoundSqlQueryCompiler>().Keyed<ICategoryBoundSqlQueryCompiler>(EFacetType.Range);

            #region __Result Services__
            builder.RegisterType<DefaultResultService>().Keyed<IResultService>("tabular");
            builder.RegisterType<MapResultService>().Keyed<IResultService>("map");

            builder.RegisterType<TabularResultSqlQueryCompiler>().Keyed<IResultSqlQueryCompiler>("tabular");
            builder.RegisterType<MapResultSqlQueryCompiler>().Keyed<IResultSqlQueryCompiler>("map");

            #endregion

            /* App Services */

            if (options.Store.UseRedisCache) {
                builder.RegisterType<Services.CachedLoadFacetService>().As<Services.ILoadFacetService>();
                builder.RegisterType<Services.CachedLoadResultService>().As<Services.ILoadResultService>();
            } else {
                builder.RegisterType<Services.LoadFacetService>().As<Services.ILoadFacetService>();
                builder.RegisterType<Services.LoadResultService>().As<Services.ILoadResultService>();
            }
            if (services != null)
                builder.Populate(services);

            var container = builder.Build();

            return container;
        }
    }
}
