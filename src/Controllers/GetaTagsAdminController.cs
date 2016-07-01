using System.Linq;
using System.Web.Mvc;
using EPiServer.Data;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;
using PagedList;

namespace Geta.Tags.Controllers
{
    [Authorize(Roles = "Administrators, WebAdmins, CmsAdmins")]
    [EPiServer.PlugIn.GuiPlugIn(Area = EPiServer.PlugIn.PlugInArea.AdminMenu, Url = "/GetaTagsAdmin/Index", DisplayName = "Geta Tags Management")]
    public class GetaTagsAdminController : Controller
    {
        public static int PageSize { get; } = 30;

        private readonly ITagRepository _tagRepository;

        public GetaTagsAdminController() : this(ServiceLocator.Current.GetInstance<ITagRepository>())
        {
        }

        public GetaTagsAdminController(ITagRepository tagRepository)
        {
            this._tagRepository = tagRepository;
        }

        public ActionResult Index(string searchString, int? page)
        {
            var pageNumber = page ?? 1;
            var tags = this._tagRepository.GetAllTags().ToList();
            ViewBag.TotalCount = tags.Count;

            if (string.IsNullOrEmpty(searchString) && (page == null || page == pageNumber))
            {
                return View(GetViewPath("Index"), tags.ToPagedList(pageNumber, PageSize));
            }

            ViewBag.SearchString = searchString;
            tags = _tagRepository.GetAllTags().Where(s => s.Name.Contains(searchString)).ToList();

            return View(GetViewPath("Index"), tags.ToPagedList(pageNumber, PageSize));
        }

        public ActionResult Edit(string tagId, int? page, string searchString)
        {
            if (tagId == null)
            {
                return HttpNotFound();
            }

            var tag = _tagRepository.GetTagById(Identity.Parse(tagId));
            if (tag == null)
            {
                return HttpNotFound();
            }

            ViewBag.Page = page;
            ViewBag.SearchString = searchString;
            return PartialView(GetViewPath("Edit"), tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Tag tag, int? page, string searchString)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var existingTag = _tagRepository.GetTagById(Identity.Parse(id));

            if (existingTag == null)
            {
                return RedirectToAction("Index", new { page = page, searchString = searchString });
            }

            existingTag.Name = tag.Name;
            existingTag.GroupKey = tag.GroupKey;

            _tagRepository.Save(existingTag);

            return RedirectToAction("Index", new { page = page, searchString = searchString });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string tagId, int? page, string searchString)
        {
            if (tagId != null)
            {
                var existingTag = _tagRepository.GetTagById(Identity.Parse(tagId));

                _tagRepository.Delete(existingTag);

                ViewBag.Page = page;
                ViewBag.SearchString = searchString;
                return RedirectToAction("Index", new { page, searchString });
            }

            return View(GetViewPath("Delete"));
        }

        private string GetViewPath(string viewName)
        {
            return Paths.ToClientResource(typeof(GetaTagsAdminController), "Views/Admin/" + viewName + ".cshtml");
        }
    }
}