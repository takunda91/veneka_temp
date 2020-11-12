using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.CardManagement.objects
{
   public class Printer
    {
        public int PrinterId { get; set; }
        public string SerialNo { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }

        public string FirmwareVersion { get; set; }

        public int BranchId { get; set; }
        public int TotalPrints { get; set; }
        public int NextClean { get; set; }
    }
}
