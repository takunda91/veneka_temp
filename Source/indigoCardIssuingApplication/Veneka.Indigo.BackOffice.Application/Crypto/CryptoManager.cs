using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.Crypto
{
    public abstract class CryptoCreator
    {
        public abstract ICryptoManager FactoryMethod(string type, string key, string aData);
    }
    public class CryptoManager: CryptoCreator
    {
        public override ICryptoManager FactoryMethod(string type,string key,string aData)
        {
            switch (type)
            {
                case "Windows": return new WindowsCrypto(key,aData);
                //case "B": return new ConcreteProductB();
                default: throw new ArgumentException("Invalid type", "type");
            }
        }
    }
}
