// Copyright (c) Geta Digital. All rights reserved.
// Licensed under MIT. See the LICENSE file in the project root for more information

using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace Geta.Tags
{
    [InitializableModule]
    public class InitializeModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.MapRoute("GetaTags", "getatags", new { action = "Index", controller = "GetaTags" });
            RouteTable.Routes.MapRoute(
                name: "GetaTagsAdmin",
                url: "getatagsadmin/{action}",
                defaults: new { controller = "GetaTagsAdmin", action = "Index" });
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}