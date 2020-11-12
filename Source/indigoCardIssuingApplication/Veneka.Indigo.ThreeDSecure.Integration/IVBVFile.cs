using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.ThreeDSecure.Integration
{
    public interface IVBVFile
    {
       bool CreateVBVFile(List<Objects.VBVCardDetails> VBVCarddetails, string path);
    }
}
