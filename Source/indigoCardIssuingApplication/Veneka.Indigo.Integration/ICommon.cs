using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration
{
    public interface ICommon
    {
        

        IDataSource DataSource { get; set; }

        /// <summary>
        /// Property for setting local file system directory for the integration layer to use
        /// </summary>
        DirectoryInfo IntegrationFolder { get; set; }
    }
}
