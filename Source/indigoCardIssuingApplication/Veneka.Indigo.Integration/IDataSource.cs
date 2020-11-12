using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.Integration
{
    public interface IDataSource
    {
        IBranchDAL BranchDAL { get; }
        ICardGeneratorDAL CardGeneratorDAL { get; }
        ICardsDAL CardsDAL { get; }
        IExportBatchDAL ExportBatchDAL { get; }
        IIssuerDAL IssuerDAL { get; }
        ILookupDAL LookupDAL { get; }
        IParametersDAL ParametersDAL { get; }
        IProductDAL ProductDAL { get; }
        ITerminalDAL TerminalDAL { get; }
        ITransactionSequenceDAL TransactionSequenceDAL { get; }

        ICustomDataDAL CustomDataDAL { get; }
        //INotificationDAL NotificationDAL { get; }

        //IFileLoaderDAL FileLoaderDAL { get; }
    }
}
