﻿using DataAccessPostgreSqlProvider;
using QuerySeadDomain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace QuerySeadDomain {

    public interface IFacetRepository : IRepository<FacetDefinition> {

    }

    public class FacetRepository : Repository<FacetDefinition>, IFacetRepository
    {
        private Dictionary<string, FacetDefinition> dictionary = null;

        public FacetRepository(DomainModelDbContext context) : base(context)
        {
        }

        protected override IQueryable<FacetDefinition> GetInclude(IQueryable<FacetDefinition> set)
        {
            return set.Include(x => x.FacetGroup)
                      .Include(x => x.FacetType)
                      .Include(x => x.Tables)
                      .Include(x => x.Clauses);
        }

        public Dictionary<string, FacetDefinition> ToDictionary()
        {
            return dictionary ?? (dictionary = GetAll().ToDictionary(x => x.FacetCode));
        }

        //public override IEnumerable<FacetDefinition> GetAll()
        //{
        //    return context.Set<FacetDefinition>().BuildFacetDefinition().ToList();
        //}

        public FacetDefinition GetByCode(string facetCode)
        {
            return ToDictionary()?[facetCode];
        }

        public IEnumerable<FacetDefinition> FindThoseWithAlias()
        {
            return GetAll().Where(p => p.Tables.Any(c => ! c.Alias.Equals("")));
            //var query = GetAll()         // source
            //  .Join(context.Tables,         // target
            //     c => c.CategoryId,          // FK
            //     cm => cm.ChildCategoryId,   // PK
            //     (c, cm) => new { Category = c, CategoryMaps = cm }) // project result
            //  .Select(x => x.Category);  // select result
        }

        public IEnumerable<FacetDefinition> GetOfType(EFacetType type)
            => Find(z => z.FacetTypeId == type);

        public dynamic GetUpperLowerBounds(FacetDefinition facet)
        {
            string sql = RangeLowerUpperSqlQueryBuilder.compile(null, facet);
            var item = QueryRow(sql, r => new { lower = r.GetDecimal(0), upper = r.GetDecimal(1) });
            return item;
        }

    }

    public static class FacetRepositoryEagerBuilder {

        public static IQueryable<FacetDefinition> BuildFacetDefinition(this IQueryable<FacetDefinition> query)
        {
            return query.Include(x => x.FacetGroup)
                        .Include(x => x.FacetType)
                        .Include(x => x.Tables)
                        .Include(x => x.Clauses);
        }

    }

}
