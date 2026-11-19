using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using ODBP.Apis.Odrc;

namespace ODBP.Test
{
    [TestClass]
    public class PagedResponseLinksFilterTest
    {
        private static PagedResponseModel<string> Execute(string path, string query, string? next, string? previous)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = path;
            httpContext.Request.QueryString = new QueryString(query);

            var model = new PagedResponseModel<string> { Results = [], Next = next, Previous = previous };
            var context = new ResultExecutingContext(
                new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
                [],
                new OkObjectResult(model),
                controller: new object());

            new PagedResponseLinksFilter().OnResultExecuting(context);

            return model;
        }

        [TestMethod]
        public void Links_zijn_relatief_naar_odbp_zonder_odrc_host_en_serverside_parameters()
        {
            var model = Execute(
                "/api/v2/documenten",
                "?publicatie=abc&sorteer=-creatiedatum&page=2",
                "https://odrc.intern.example/api/v2/documenten?page=3&publicatie=abc&publicatiestatus=gepubliceerd&isGereedVoorPublicatie=true&sorteer=-creatiedatum",
                "https://odrc.intern.example/api/v2/documenten?page=1&publicatie=abc&publicatiestatus=gepubliceerd&sorteer=-creatiedatum");

            Assert.AreEqual("/api/v2/documenten?page=3&publicatie=abc&sorteer=-creatiedatum", model.Next);
            Assert.AreEqual("/api/v2/documenten?page=1&publicatie=abc&sorteer=-creatiedatum", model.Previous);
        }

        [TestMethod]
        public void Ontbrekende_page_in_de_odrc_link_is_pagina_1()
        {
            var model = Execute(
                "/api/v2/onderwerpen",
                "?page=2",
                null,
                "https://odrc.intern.example/api/v2/onderwerpen?publicatiestatus=gepubliceerd");

            Assert.AreEqual("/api/v2/onderwerpen?page=1", model.Previous);
        }

        [TestMethod]
        public void Geen_of_ongeldige_link_wordt_null()
        {
            var model = Execute(
                "/api/v2/onderwerpen",
                "",
                null,
                "https://odrc.intern.example/api/v2/onderwerpen?page=abc");

            Assert.IsNull(model.Next);
            Assert.IsNull(model.Previous);
        }
    }
}
