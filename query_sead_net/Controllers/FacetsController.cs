﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuerySeadDomain;

namespace query_sead_net.Controllers
{
    [Route("api/[controller]")]
    public class FacetsController : Controller
    {
        public IUnitOfWork Context { get; private set; }

        public FacetsController(IUnitOfWork context)
        {
            Context = context;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<FacetDefinition> Get()
        {
            return Context.Facets.GetAll().ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public FacetDefinition Get(int id)
        {
            return Context.Facets.Get(id);
        }

        // POST api/values
        [HttpPost]
        public int Post([FromBody]FacetsConfig2 data)
        {
            return 0;
        }

    }
}
