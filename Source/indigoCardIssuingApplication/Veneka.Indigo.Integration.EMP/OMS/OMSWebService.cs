using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.EMP.OMS
{
    public class OMSWebService
    {
        private static readonly ILog _cmsLog = LogManager.GetLogger(General.CMS_LOGGER);

        //private readonly OMSServiceValidated _omsServices;
     

        public OMSWebService(Parameters parameters, string connectionString)
        {
            //this._parms = parameters;
            //Veneka.Module.TranzwareCompassPlusOMSI.ServicesValidated.Protocol protocol = Veneka.Module.TranzwareCompassPlusOMSI.ServicesValidated.Protocol.HTTP;

            //switch (parameters.Protocol.GetValueOrDefault(Parameters.protocol.HTTP))
            //{
            //    case Parameters.protocol.HTTP: protocol = Veneka.Module.TranzwareCompassPlusOMSI.ServicesValidated.Protocol.HTTP; break;
            //    case Parameters.protocol.HTTPS: protocol = Veneka.Module.TranzwareCompassPlusOMSI.ServicesValidated.Protocol.HTTPS; break;
            //    default: break;
            //}

            //_omsServices = new OMSServiceValidated(); //protocol, parameters.Address, parameters.Port, parameters.Path,
            //                                          //parameters.Timeout, connectionString, General.CMS_LOGGER);

            //_lookupDAL = new LookupDAL(connectionString);
        }
    }
}
