using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using Geta.Tags.EditorDescriptors;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;
using PagedList;

namespace Geta.Tags.Controllers
{
    [Authorize(Roles = "Administrators, WebAdmins, CmsAdmins")]
    [EPiServer.PlugIn.GuiPlugIn(Area = EPiServer.PlugIn.PlugInArea.AdminMenu, Url = "/GetaTagsAdmin", DisplayName = "Geta Tags Management")]
    public class GetaTagsAdminController : Controller
    {
        public static int PageSize { get; } = 30;

        private readonly ITagRepository _tagRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITagEngine _tagEngine;

        public GetaTagsAdminController() : this(ServiceLocator.Current.GetInstance<ITagRepository>(), ServiceLocator.Current.GetInstance<IContentRepository>(), ServiceLocator.Current.GetInstance<ITagEngine>())
        {
        }

        public GetaTagsAdminController(ITagRepository tagRepository, IContentRepository contentRepository, ITagEngine tagEngine)
        {
            this._tagRepository = tagRepository;
            this._contentRepository = contentRepository;
            this._tagEngine = tagEngine;
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

            var existingTagName = existingTag.Name;
            var contentReferencesFromTag = _tagEngine.GetContentReferencesByTags(existingTagName);

            foreach (var item in contentReferencesFromTag)
            {
                var pageFromRepository = _contentRepository.Get<IContent>(item) as PageData;

                var clone = pageFromRepository.CreateWritableClone();
                var tagAttributes = clone.GetType().GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(GetaTagsAttribute)) && prop.PropertyType == typeof(string));

                foreach (var tagAttribute in tagAttributes)
                {
                    var tags = tagAttribute.GetValue(clone) as string;
                    if (tags.IsNullOrEmpty()) continue;

                    IList<string> pageList = tags.Split(',').ToList<string>();
                    int indexItemToRemove = pageList.IndexOf(existingTagName);

                    if (indexItemToRemove == -1) continue;
                    pageList.RemoveAt(indexItemToRemove);

                    var newPageTags = "";
                    foreach (var listItem in pageList)
                    {
                        newPageTags += listItem + ",";
                    }

                    var allTags = newPageTags + tag.Name;
                    tagAttribute.SetValue(clone, allTags);
                }

                _contentRepository.Save(clone, SaveAction.Publish, AccessLevel.NoAccess);
            }

            _tagRepository.Delete(existingTag);

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