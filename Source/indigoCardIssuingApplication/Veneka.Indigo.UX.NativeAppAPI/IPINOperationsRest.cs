﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [ServiceContract]
    public interface IPINOperationsRest
    {
        [OperationContract]
        [WebGet(UriTemplate = "/get/{number}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string LogonPINRest(string number);
    }
}
