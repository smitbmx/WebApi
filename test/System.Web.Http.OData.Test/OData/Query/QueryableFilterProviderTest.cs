﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.OData.TestCommon.Models;
using Microsoft.TestCommon;

namespace System.Web.Http.OData.Query
{
    public class QueryableFilterProviderTest
    {
        [Theory]
        [InlineData("GetQueryable")]
        [InlineData("GetGenericQueryable")]
        public void GetFilters_ReturnsQueryableFilter_ForQueryableActions(string actionName)
        {
            HttpConfiguration config = new HttpConfiguration();
            HttpControllerDescriptor controllerDescriptor = new HttpControllerDescriptor(config, "FilterProviderTest", typeof(FilterProviderTestController));
            HttpActionDescriptor actionDescriptor = new ReflectedHttpActionDescriptor(controllerDescriptor, typeof(FilterProviderTestController).GetMethod(actionName));

            FilterInfo[] filters = new QueryableFilterProvider().GetFilters(config, actionDescriptor).ToArray();

            Assert.Equal(1, filters.Length);
            Assert.Equal(FilterScope.Global, filters[0].Scope);
            QueryableAttribute filter = Assert.IsType<QueryableAttribute>(filters[0].Instance);
        }

        [Theory]
        [InlineData("GetEnumerable")]
        [InlineData("GetGenericEnumerable")]
        [InlineData("GetArray")]
        [InlineData("GetList")]
        [InlineData("GetObject")]
        [InlineData("GetGenericQueryableWithODataQueryOption")]
        [InlineData("GetGenericQueryableWithODataQueryOptionOfT")]
        [InlineData("GetGenericQueryableWithODataQueryOption2")]
        public void GetFilters_ReturnsEmptyCollection_ForNonQueryableActions(string actionName)
        {
            HttpConfiguration config = new HttpConfiguration();
            HttpControllerDescriptor controllerDescriptor = new HttpControllerDescriptor(config, "FilterProviderTest", typeof(FilterProviderTestController));
            HttpActionDescriptor actionDescriptor = new ReflectedHttpActionDescriptor(controllerDescriptor, typeof(FilterProviderTestController).GetMethod(actionName));

            FilterInfo[] filters = new QueryableFilterProvider().GetFilters(config, actionDescriptor).ToArray();

            Assert.Empty(filters);
        }

        [Fact]
        public void GetFilters_AppliesResultLimit()
        {
            HttpConfiguration config = new HttpConfiguration();
            HttpControllerDescriptor controllerDescriptor = new HttpControllerDescriptor(config, "FilterProviderTest", typeof(FilterProviderTestController));
            HttpActionDescriptor actionDescriptor = new ReflectedHttpActionDescriptor(controllerDescriptor, typeof(FilterProviderTestController).GetMethod("GetQueryable"));

            FilterInfo[] filters = new QueryableFilterProvider() { ResultLimit = 25 }.GetFilters(config, actionDescriptor).ToArray();

            Assert.Equal(1, filters.Length);
            Assert.Equal(FilterScope.Global, filters[0].Scope);
            QueryableAttribute filter = Assert.IsType<QueryableAttribute>(filters[0].Instance);
            Assert.Equal(25, filter.ResultLimit);
        }

        [Theory]
        [InlineData(typeof(IEnumerable), false)]
        [InlineData(typeof(IQueryable), true)]
        [InlineData(typeof(IEnumerable<Customer>), false)]
        [InlineData(typeof(IQueryable<Customer>), true)]
        [InlineData(typeof(object), false)]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(List<Customer>), false)]
        [InlineData(typeof(Customer[]), false)]
        public void IsIQueryable_ReturnsWhetherTypeIsIQueryable(Type type, bool isIQueryable)
        {
            Assert.Equal(isIQueryable, QueryableFilterProvider.IsIQueryable(type));
        }
    }

    public class FilterProviderTestController : ApiController
    {
        public IEnumerable GetEnumerable()
        {
            return null;
        }

        public IQueryable GetQueryable()
        {
            return null;
        }

        public IEnumerable<string> GetGenericEnumerable()
        {
            return null;
        }

        public IQueryable<string> GetGenericQueryable()
        {
            return null;
        }

        public string[] GetArray()
        {
            return null;
        }

        public List<string> GetList()
        {
            return null;
        }

        public object GetObject()
        {
            return null;
        }

        public IQueryable<string> GetGenericQueryableWithODataQueryOption(ODataQueryOptions queryOptions)
        {
            return null;
        }

        public IQueryable GetGenericQueryableWithODataQueryOptionOfT(ODataQueryOptions<string> queryOptions)
        {
            return null;
        }

        public IQueryable<string> GetGenericQueryableWithODataQueryOption2(string s, ODataQueryOptions queryOptions)
        {
            return null;
        }

        [Queryable(ResultLimit = 100)]
        public IQueryable GetQueryableWithFilterAttribute()
        {
            return null;
        }
    }
}