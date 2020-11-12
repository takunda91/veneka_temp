using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndigoMigrationToolkit.Versions
{
    public sealed class v2000 : IndigoVersionInfo
    {
        public v2000() : base("v2000") { }        

        public override string Name
        {
            get
            {
                return "v2.0.0.0";
            }
        }

        public override string FolderName { get { return "v2000"; } }

        public override bool ValidateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
