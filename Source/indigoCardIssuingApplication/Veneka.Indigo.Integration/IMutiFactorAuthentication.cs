
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration;

namespace Veneka.Indigo.Integration
{
  public  interface IMutiFactorAuthentication:ICommon
    {
        bool VerifyChallenge(Config.IConfig config, string username, Dictionary<string, string> additionalData,string token,out string responseMessage);

        bool SendChallenge(Config.IConfig config, string username, Dictionary<string, string> additionalData, out string responseMessage);
        
    }
}

