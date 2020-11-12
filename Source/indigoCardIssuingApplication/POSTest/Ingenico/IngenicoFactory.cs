using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Module.ISO8583;
using Veneka.Module.ISO8583.MessageFactory;

namespace POSTest.Ingenico
{
    public class IngenicoFactory : AbstractFactory
    {
        public override I8583 Create(byte[] message)
        {
            return new ISO8583(message, IngenicoDataList.Create());
        }
    }
}
