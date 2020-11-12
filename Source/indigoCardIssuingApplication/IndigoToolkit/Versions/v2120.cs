using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndigoMigrationToolkit.Versions
{
    public sealed class v2120 : IndigoVersionInfo
    {
        public v2120() : base("v2120") { }

        public override string Name
        {
            get
            {
                return "v2.1.2.0";
            }
        }

        public override string FolderName { get { return "v2120"; } }

        public override bool ValidateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
