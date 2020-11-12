using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.Common
{
    public static class CardNumberGenerator
    {
        public static string GenerateCardNumber(CardObject cardobj, int sequenceNumber, int cardLength)
        {
            //RAB - not sure where the unique number comes from.           
            string cardNumber = cardobj.BIN + cardobj.SubProductCode;
            cardNumber += sequenceNumber.ToString()
                                        .PadLeft(cardLength - cardNumber.Length - 1, '0');
            cardNumber += CheckDigit(cardNumber);

            if (cardNumber.ToString().Length > cardLength)
                throw new FormatException("Generated card number length is greater than the allowed length for the card.");
            else if (cardNumber.ToString().Length < cardLength)
                throw new FormatException("Generated card number length is smaller than the length for the card.");

            return cardNumber;
        }

        private static string CheckDigit(string CardNumber)
        {
            int cardnumbertotal = 0;
            int i = 1;
            char[] charArray = CardNumber.ToCharArray();
            // for reading numbers from right to left
            Array.Reverse(charArray);

            // reading each character
            foreach (char ch in charArray)
            {
                // for getting alternative number
                if (i % 2 != 0)
                {
                    int mul = int.Parse(ch.ToString()) * 2;
                    int total = 0;
                    foreach (char c in mul.ToString())
                    {
                        total += int.Parse(c.ToString());
                    }
                    cardnumbertotal += total;
                }
                else
                {
                    cardnumbertotal += int.Parse(ch.ToString());
                }
                i++;
            }

            // finding near value for 10 factor
            int k = cardnumbertotal / 10;
            int checkdigit = 0;
            if (cardnumbertotal % 10 != 0)
            {
                checkdigit = ((k + 1) * 10) - cardnumbertotal;
            }

            return checkdigit.ToString();
        }
    }
}
