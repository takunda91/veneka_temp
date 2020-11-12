using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndigoMigrationToolkit.Versions
{
    public sealed class v2110 : IndigoVersionInfo
    {
        public v2110() : base("v2110") { }

        public override string Name
        {
            get
            {
                return "v2.1.1.0";
            }
        }

        public override string FolderName { get { return "v2110"; } }

        public override bool ValidateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
