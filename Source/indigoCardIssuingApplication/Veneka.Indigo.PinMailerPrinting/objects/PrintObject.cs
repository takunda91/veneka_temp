using System.Linq;
using System.Text;

namespace Veneka.Indigo.PinMailerPrinting.objects
{
    public class PrintObject
    {
        private readonly string _branchName;
        private readonly string _cardNumber;
        private readonly string _customerName;
        private readonly string _pin;
        private readonly string _pinWords;

        public PrintObject(string pCustomerName, string pCardNumber, string pBranchName, string pPin)
        {
            _customerName = pCustomerName;
            _cardNumber = formatCardNumber(pCardNumber);
            _branchName = pBranchName;
            _pin = pPin;
            _pinWords = pinInWords(pPin);
        }

        public string CustomerName
        {
            get { return _customerName; }
        }

        public string CardNumber
        {
            get { return _cardNumber; }
        }

        public string BranchName
        {
            get { return _branchName; }
        }

        public string PIN
        {
            get { return _pin; }
        }

        public string PINWords
        {
            get { return _pinWords; }
        }

        private string formatCardNumber(string cardNumber)
        {
            var fCN = new StringBuilder();
            fCN.Append(cardNumber.Substring(0, 4));
            fCN.Append(" **** **** ");
            fCN.Append(cardNumber.Substring(cardNumber.Length - 4));
            return fCN.ToString();
        }

        private string pinInWords(string pin)
        {
            string pinWords = "";

            for (int i = 0; i < pin.Length; i++)
            {
                char number = pin.ElementAt(i);
                switch (number)
                {
                    case '0':
                        pinWords += "ZERO ";
                        break;
                    case '1':
                        pinWords += "ONE ";
                        break;
                    case '2':
                        pinWords += "TWO ";
                        break;
                    case '3':
                        pinWords += "THREE ";
                        break;
                    case '4':
                        pinWords += "FOUR ";
                        break;
                    case '5':
                        pinWords += "FIVE ";
                        break;
                    case '6':
                        pinWords += "SIX ";
                        break;
                    case '7':
                        pinWords += "SEVEN ";
                        break;
                    case '8':
                        pinWords += "EIGHT ";
                        break;
                    case '9':
                        pinWords += "NINE ";
                        break;
                }
            }
            return pinWords;
        }
    }
}