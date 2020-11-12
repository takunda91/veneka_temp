using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndigoMigrationToolkit.Versions
{
    public sealed class v2100 : IndigoVersionInfo
    {
        public v2100() : base("v2100") { }

        public override string Name
        {
            get
            {
                return "v2.1.0.0";
            }
        }

        public override string FolderName { get { return "v2100"; } }

        public override bool ValidateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
