
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.COMS.DataSource.LocalDAL;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.COMS.DataSource
{
    public class LocalDataSource : IDataSource
    {
        public LocalDataSource()
        {
            BranchDAL = new BranchDAL();
            CardGeneratorDAL = new CardGeneratorDAL();
            CardsDAL = new CardsDAL();
            ExportBatchDAL = new ExportBatchDAL();
            IssuerDAL = new IssuerDAL();
            LookupDAL = new Veneka.Indigo.COMS.DataSource.LocalDAL.LookupDAL();
            ParametersDAL = new ParametersDAL();
            ProductDAL = new ProductDAL();
            TerminalDAL = new TerminalDAL();
            TransactionSequenceDAL = new TransactionSequenceDAL();
            CustomDataDAL = new CustomDataDAL();
        }
        public ICustomDataDAL CustomDataDAL { get; private set; }

        public IBranchDAL BranchDAL { get; private set; }

        public ICardGeneratorDAL CardGeneratorDAL { get; private set; }

        public ICardsDAL CardsDAL { get; private set; }

        public IExportBatchDAL ExportBatchDAL { get; private set; }

        public IIssuerDAL IssuerDAL { get; private set; }

        public ILookupDAL LookupDAL { get; private set; }

        public IParametersDAL ParametersDAL { get; private set; }

        public IProductDAL ProductDAL { get; private set; }

        public ITerminalDAL TerminalDAL { get; private set; }

        public ITransactionSequenceDAL TransactionSequenceDAL { get; private set; }

    }
}