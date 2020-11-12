using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core.CardManagement;
using Veneka.Indigo.COMS.Core.HardwareSecurityModule;
using Veneka.Indigo.COMS.Core.Indigo;
using Veneka.Indigo.COMS.Core.Terminal;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.COMS.Core
{
    [ServiceContract(Namespace = Constants.IndigoComsCallbackURL)]
    public interface IComsCallback: IBranchDAL, ICardsDAL, ICardGeneratorDAL, IExportBatchDAL, IIssuerDAL, ILookupDAL, IParametersDAL, IProductDAL, ITerminalDAL, ITransactionSequenceDAL,ICustomDataDAL
    {
        
    }
}
