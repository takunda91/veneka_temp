using System;

namespace indigoCardIssuingWeb.utility
{
    public static class ValidationUtility
    {
        /// <summary>
        /// This method returns true if the specified input string only contains Numeric characters.
        /// </summary>
        public static bool ValidateNumericsOnly(string input)
        {
            foreach (char c in input)
            {
                if (!Char.IsNumber(c))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This method returns true if the specified input string only contains AlpahNumeric characters.
        /// </summary>
        public static bool ValidateAlphaNumericOnly(string input)
        {
            foreach (char c in input)
            {
                if (!Char.IsLetterOrDigit(c))
                {
                    return false;
                }
            }
            
            return true;
        }
        /// <summary>
        /// This method returns true if the specified input string only contains Alpah characters.
        /// </summary>
        public static bool ValidateAlphOnly(string input)
        {
            foreach (char c in input)
            {
                if (!Char.IsLetter(c))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool HasInvalidChars(String text)
        {
            var inValidChars = new[]
                {
                    '!', '@', '#', '$', '%','-',
                    '^', '&', '*', '(', ')', '\'', '\"', '{', '}', '<', '>',
                    '/', '\\', ':', ';', '[', '}'
                };


            foreach (char c in inValidChars)
            {
                if (text.Contains(c.ToString()))
                    return true;
            }
            return false;
        }

    }
}