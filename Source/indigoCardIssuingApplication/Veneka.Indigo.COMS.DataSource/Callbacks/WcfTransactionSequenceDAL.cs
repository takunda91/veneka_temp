using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.COMS.DataSource.Callbacks
{
    public class WcfTransactionSequenceDAL : ISequenceGenerator, ITransactionSequenceDAL
    {
        private readonly IComsCallback _proxy;
        public WcfTransactionSequenceDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }
       public int NextSequenceNumber(string sequenceName, ResetPeriod resetPeriod)
        {
            return _proxy.NextSequenceNumber(sequenceName, resetPeriod);
        }

       public long NextSequenceNumberLong(string sequenceName, ResetPeriod resetPeriod)
        {
            return _proxy.NextSequenceNumberLong(sequenceName, resetPeriod);
        }

        public void Dispose()
        {
            _proxy.Dispose();
        }
    }
}
