using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndigoFileLoader.Modules.Extensibility;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Common.Logging;

namespace IndigoFileLoader.Modules
{
    public sealed class IndigoFileReaderModule
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IndigoFileReaderModule));
        private CompositionContainer _container;

        [Import(typeof(ICardFileReader))]
        public ICardFileReader cardFileReader;

        public IndigoFileReaderModule()
        {
            try
            {
                //An aggregate catalog that combines multiple catalogs
                var catalog = new AggregateCatalog();
                //Adds all the parts found in the same assembly as the Program class
                //TODO: Should get this from config file
                catalog.Catalogs.Add(new DirectoryCatalog("C:\\veneka\\indigo_group\\modules\\FileloaderModules"));

                //Create the CompositionContainer with the parts in the catalog
                _container = new CompositionContainer(catalog);

                //Fill the imports of this object            
                this._container.ComposeParts(this);
            }
            catch (Exception ex)//TODO: Look at better exception handling...
            {
                if (log.IsDebugEnabled)
                {
                    log.Fatal(ex);
                }

                throw new Exception("An error has occured while loading the file reader module, please verify that the module is present.");
            }
        }
    }
}
