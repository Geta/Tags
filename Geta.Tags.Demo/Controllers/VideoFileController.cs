using System.Web.Mvc;
using Geta.Tags.Demo.Models.Media;
using Geta.Tags.Demo.Models.ViewModels;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using System;
using EPiServer.Core;

namespace Geta.Tags.Demo.Controllers
{
    /// <summary>
    /// Controller for the video file.
    /// </summary>
    public class VideoFileController : PartialContentController<VideoFile>
    {
        private readonly UrlResolver _urlResolver;

        public VideoFileController(UrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        /// <summary>
        /// The index action for the video file. Creates the view model and renders the view.
        /// </summary>
        /// <param name="currentContent">The current video file.</param>
        public override ActionResult Index(VideoFile currentContent)
        {
            var model = new VideoViewModel
            {
                Url = _urlResolver.GetUrl(currentContent.ContentLink),
                PreviewImageUrl = ContentReference.IsNullOrEmpty(currentContent.PreviewImage) ? String.Empty : _urlResolver.GetUrl(currentContent.PreviewImage),
            };

            return PartialView(model);
        }
    }
}
