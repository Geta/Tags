using System.Linq;
using EPiServer.Core.Transfer;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.Enterprise;
using EPiServer.Enterprise.Transfer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Geta.Tags.Models;

namespace Geta.Tags
{
    /// <summary>
    /// Module to transfer Tags content to mirrored servers when in a full staging/delivery configuration
    /// Source: EPiRobots - http://epirobots.codeplex.com/
    /// </summary>
    [InitializableModule, ModuleDependency(typeof(DataInitialization)), ModuleDependency(typeof(DynamicDataTransferHandler))]
    public class TagsTransferModule : IInitializableModule
    {
        private Injected<IDataExportEvents> DataExportEvents { get; set; }

        public void Initialize(InitializationEngine context)
        {
            DataExportEvents.Service.ContentExporting += DataExportEvents_ContentExporting;
        }

        private void DataExportEvents_ContentExporting(ITransferContext transferContext, ContentExportingEventArgs e)
        {
            var exporter = transferContext as ITransferHandlerContext;
            if (exporter != null && exporter.TransferType == TypeOfTransfer.MirroringExporting)
            {
                var ddsHandler = exporter.TransferHandlers.Single(p => p.GetType() == typeof(DynamicDataTransferHandler)) as DynamicDataTransferHandler;

                var store = typeof(Tag).GetStore();
                var externalId = store.GetIdentity().ExternalId;
                var storeName = store.Name;

                if (ddsHandler != null)
                {
                    ddsHandler.AddToExport(externalId, storeName);
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            DataExportEvents.Service.ContentExporting -= DataExportEvents_ContentExporting;
        }
    }
}