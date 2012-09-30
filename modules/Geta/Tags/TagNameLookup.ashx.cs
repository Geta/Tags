using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Geta.Tags.Implementations;
using Geta.Tags.Interfaces;
using Geta.Tags.Models;

namespace Geta.Tags.modules.Geta.Tags
{
    public class TagNameLookup : IHttpHandler
    {
        public ITagService TagService { get; set; }

        public TagNameLookup()
        {
            this.TagService = new TagService(new TagRepository());
        }

        public void ProcessRequest(HttpContext context)
        {
            string termQuery = context.Request.QueryString["term"];
            context.Response.ClearContent();
            context.Response.ContentType = "application/json; charset=UTF-8";

            if (string.IsNullOrEmpty(termQuery))
            {
                context.Response.End();
                return;
            }

            List<Tag> matchingTerms = this.TagService.GetAllTags()
                .Where(t => t.Name.ToLower().StartsWith(termQuery.ToLower()))
                .OrderBy(t => t.Name)
                .Take(10)
                .ToList();

            if (matchingTerms.Count == 0)
            {
                context.Response.End();
                return;
            }

            var termsForAutoComplete = matchingTerms.Select(term => new
            {
                value = term.Name,
                label = term.Name
            });

            var jsonSerializer = new JavaScriptSerializer();

            context.Response.Write(jsonSerializer.Serialize(termsForAutoComplete));

            context.Response.End();
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}