using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.NotificationService.DAL
{
    public class LocalDataSource : IDataSource
    {
        public LocalDataSource(string connectionString)
        {
            NotificationDAL = new NotificationDAL(connectionString);
        }
        public IBranchDAL BranchDAL => throw new NotImplementedException();

        public ICardGeneratorDAL CardGeneratorDAL => throw new NotImplementedException();

        public ICardsDAL CardsDAL => throw new NotImplementedException();

        public IExportBatchDAL ExportBatchDAL => throw new NotImplementedException();

        public IIssuerDAL IssuerDAL => throw new NotImplementedException();

        public ILookupDAL LookupDAL => throw new NotImplementedException();

        public IParametersDAL ParametersDAL => throw new NotImplementedException();

        public IProductDAL ProductDAL => throw new NotImplementedException();

        public ITerminalDAL TerminalDAL => throw new NotImplementedException();

        public ITransactionSequenceDAL TransactionSequenceDAL => throw new NotImplementedException();

        public INotificationDAL NotificationDAL { get; private set; }

        public IFileLoaderDAL FileLoaderDAL { get; private set; }

        public ICustomDataDAL CustomDataDAL { get; private set; }
    }
}
