using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [ServiceContract]
    public interface INativeAppRest: IGeneralRest, IPINOperationsRest, ICardPrintingRest
    {
    }
}
