using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.COMS.DataSource.Callbacks
{
    public class WcfLookupDAL: ILookupDAL
    {
        private readonly IComsCallback _proxy;
        public WcfLookupDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }

       
        public string LookupBranchCode(int branchId)
        {
            return _proxy.LookupBranchCode(branchId);
        }

        public string LookupBranchName(int branchId)
        {
            return _proxy.LookupBranchName(branchId);
        }

        public string LookupCurrency(int currencyId)
        {
            return _proxy.LookupCurrency(currencyId);
        }

        public int LookupCurrency(string ccy)
        {
            return _proxy.LookupCurrency(ccy);
        }

        public string LookupCurrencyISONumericCode(int currencyId)
        {
            return _proxy.LookupCurrencyISONumericCode(currencyId);
        }

        public string LookupEmpBranchCode(int branchId)
        {
            return _proxy.LookupEmpBranchCode(branchId);
        }
    }
}
