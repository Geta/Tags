// Copyright (c) Geta Digital. All rights reserved.
// Licensed under MIT. See the LICENSE file in the project root for more information

using System.Linq;
using System.Web.Mvc;
using EPiServer.ServiceLocation;
using Geta.Tags.Interfaces;

namespace Geta.Tags.Controllers
{
    public class GetaTagsController : Controller
    {
        private readonly ITagService _tagService;

        public GetaTagsController() : this (ServiceLocator.Current.GetInstance<ITagService>())
        {
        }

        public GetaTagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        public JsonResult Index(string term, string groupKey = null)
        {
            var normalized = Normalize(term);
            var tags = _tagService.GetAllTags();

            if (IsNotEmpty(normalized))
            {
                tags = tags.Where(t => t.Name.ToLower().StartsWith(normalized.ToLower()));

                if (IsNotEmpty(groupKey))
                {
                    tags = tags.Where(t => t.GroupKey.Equals(groupKey));
                }
            }

            var items = tags.OrderBy(t => t.Name)
                .Select(t => t.Name)
                .Take(10)
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        private static string Normalize(string term)
        {
            return string.IsNullOrWhiteSpace(term) ? string.Empty : term;
        }

        private static bool IsNotEmpty(string name)
        {
            return !string.IsNullOrEmpty(name);
        }
    }
}