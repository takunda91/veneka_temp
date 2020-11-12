using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Config
{
   
    public interface IConfig
    {
        Guid InterfaceGuid { get; }
        void LoadConfig(System.Data.DataRow configRow);
    }
}
