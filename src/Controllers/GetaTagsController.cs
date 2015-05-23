using System.Linq;
using System.Web.Mvc;
using Geta.Tags.Implementations;
using Geta.Tags.Interfaces;

namespace Geta.Tags.Controllers
{
    public class GetaTagsController : Controller
    {
        private readonly ITagService _tagService;

        public GetaTagsController()
        {
            _tagService = new TagService();
        }

        public JsonResult Index(string term)
        {
            var normalized = Normalize(term);
            var tags = _tagService.GetAllTags();

            if (IsNotEmpty(normalized))
            {
                tags = tags.Where(t => t.Name.ToLower().StartsWith(normalized.ToLower()));
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