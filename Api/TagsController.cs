using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Geta.Tags.Implementations;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;

namespace Geta.Tags.Api
{
    public class TagsController : ApiController
    {
        private readonly ITagService _tagService;

        public TagsController() : this(new TagService())
        {
        }

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        public HttpResponseMessage Get(string name)
        {
            var normalizedName = Normalize(name);
            var tags = _tagService.GetAllTags();

            if (IsNotEmpty(normalizedName))
            {
                tags = tags.Where(t => t.Name.ToLower().StartsWith(normalizedName.ToLower()));
            }

            var items = tags.OrderBy(t => t.Name)
                .Take(10)
                .ToList()
                .Select(ToAutoComplete);

            return Request.CreateResponse(HttpStatusCode.OK, items);
        }

        private static string Normalize(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }
            return name.TrimEnd('*');
        }

        private static bool IsNotEmpty(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        private static object ToAutoComplete(Tag tag)
        {
            return new { name = tag.Name, id = tag.Name };
        }
    }
}