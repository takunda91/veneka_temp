using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    public interface IFile
    {
        string IssuerCode { get; set; }
        string FileIdentifier { get; set; }
        Issuer FileIssuer { get; set; }
    }
}
