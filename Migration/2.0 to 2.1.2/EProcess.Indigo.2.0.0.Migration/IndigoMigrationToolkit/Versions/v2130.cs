using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndigoMigrationToolkit.Versions
{
    public sealed class v2130 : IndigoVersionInfo
    {
        public v2130() : base("v2130") { }

        public override string Name
        {
            get
            {
                return "v2.1.3.0";
            }
        }

        public override string FolderName { get { return "v2130"; } }

        public override bool ValidateDatabase()
        {
            //newDB.Tables.Count == 155 &&
            //                spCount == 355 &&
            //                viewCount == 13 &&
            //                newDB.SymmetricKeys.Count == 3 &&
            //                newDB.Certificates.Count == 3)
            throw new NotImplementedException();
        }
    }
}
