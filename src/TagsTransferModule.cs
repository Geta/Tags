using System;
using System.Linq;
using EPiServer.Core.Transfer;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.Enterprise;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using Geta.Tags.Models;

namespace Geta.Tags
{
    /// <summary>
    /// Module to transfer the robots.txt content to mirrored servers when in a full staging/delivery configuration
    /// Source: EPiRobots - http://epirobots.codeplex.com/
    /// </summary>
    [InitializableModule, ModuleDependency(typeof(DataInitialization)), ModuleDependency(typeof(DynamicDataTransferHandler))]
    public class TagsTransferModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            DataExporter.Exporting += this.DataExporter_Exporting;
        }

        public void Uninitialize(InitializationEngine context)
        {
            DataExporter.Exporting -= this.DataExporter_Exporting;
        }

        public void Preload(string[] parameters)
        {
        }

        private void DataExporter_Exporting(object sender, EventArgs e)
        {
            var exporter = sender as DataExporter;
            if (exporter != null && exporter.TransferType == TypeOfTransfer.MirroringExporting)
            {
                var ddsHandler = (sender as DataExporter).TransferHandlers.Where(p => p.GetType() == typeof(DynamicDataTransferHandler)).Single() as DynamicDataTransferHandler;

                var store = typeof(Tag).GetStore();
                var externalId = store.GetIdentity().ExternalId;
                var storeName = store.Name;

                if (ddsHandler != null)
                {
                    ddsHandler.AddToExport(externalId, storeName);
                }
            }
        }
    }
}