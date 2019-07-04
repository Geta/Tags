using EPiServer.Core;

namespace Geta.Tags.Demo.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
