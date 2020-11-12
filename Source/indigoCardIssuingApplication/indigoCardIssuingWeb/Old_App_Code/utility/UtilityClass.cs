using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace indigoCardIssuingWeb.utility
{
    public enum PageLayout
    {
        CREATE,
        READ,
        UPDATE,
        DELETE,
        CONFIRM_CREATE,
        CONFIRM_UPDATE,
        CONFIRM_DELETE,
        RESET,
        PREIVEW,
        CONFIRM_ACTIVATE,
        CONFIRM_APPROVE,
        CONFIRM_REJECT
    }

    public enum ResultNavigation
    {
        FIRST,
        NEXT,
        PREVIOUS,
        LAST
    }

    public sealed class UtilityClass
    {
        
        /// <summary>
        /// This method will mask the pan with the specifed mask character.
        /// </summary>
        /// <param name="clearPAN"></param>
        /// <param name="maskCharacter"></param>
        /// <returns></returns>
        public static string DisplayPartialPAN(string clearPAN, char maskCharacter)
        {
            if (clearPAN != null && clearPAN.Length > 10)
            {
                return clearPAN.Substring(0, 6) + maskCharacter.ToString().PadRight(clearPAN.Length - 10, maskCharacter) +
                       clearPAN.Substring(clearPAN.Length - 4, 4);
            }

            return "********";
        }

        /// <summary>
        /// This method will add in dashes to the PAN to make it more readable.
        /// </summary>
        /// <param name="pan"></param>
        /// <returns></returns>
        public static string FormatPAN(string clearPAN)
        {
            if (clearPAN != null && clearPAN.Length > 10)
            {
                return clearPAN.Substring(0, 6) + "-" + clearPAN.Substring(6, clearPAN.Length - 10) + "-" +
                       clearPAN.Substring(clearPAN.Length - 4, 4);
            }

            return clearPAN;
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            s = s.ToLower();
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        /// Formats a list itme text as code - name.
        /// </summary>
        /// <param name="name">Name of item example: issuer name</param>
        /// <param name="code">Code of item example: issuer code</param>
        /// <param name="Id">a unique id for the item.</param>
        /// <returns></returns>
        public static System.Web.UI.WebControls.ListItem FormatListItem<t>(string name, string code, t Id)
        {
            return FormatListItem(name, code, Id.ToString());
        }

        /// <summary>
        /// Formats a list itme text as code - name.
        /// </summary>
        /// <param name="name">Name of item example: issuer name</param>
        /// <param name="code">Code of item example: issuer code</param>
        /// <param name="Id">a unique id for the item.</param>
        /// <returns></returns>
        public static System.Web.UI.WebControls.ListItem FormatListItem(string name, string code, string Id)
        {
            if (String.IsNullOrWhiteSpace(Id))
                throw new ArgumentNullException("Id", "May not be null or empty.");

            return new System.Web.UI.WebControls.ListItem(FormatNameAndCode(name, code), Id);
        }

        /// <summary>
        /// Formats Name and Code pairs, for example branch name and code.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string FormatNameAndCode(string name, string code)
        {
            if (!String.IsNullOrWhiteSpace(code) && !String.IsNullOrWhiteSpace(name))
                return String.Format("{0} - {1}", code, name);
            else if (!String.IsNullOrWhiteSpace(name))
                return name;
            else if (!String.IsNullOrWhiteSpace(code))
                return code;
            else
                throw new ArgumentNullException("name and/or code", "Cannot be null or empty");
        }

        public static string GetLang(int selectedLang)
        {
            string lang;
            switch (selectedLang)
            {
                case 0:
                    lang = "en";
                    break;
                case 1:
                    lang = "fr";
                    break;
                case 2:
                    lang = "pt";
                    break;
                case 3:
                    lang = "es";
                    break;
                default:
                    lang = "en";
                    break;
            }// end switch (selectedLang)

            return lang;
        }

        /// <summary>
        /// This method creates a string of key value pairs for use with the primefaces ui PickList.
        /// </summary>
        /// <typeparam name="k"></typeparam>
        /// <typeparam name="v"></typeparam>
        /// <param name="list"></param>
        /// <param name="KeyValueSeperator"></param>
        /// <param name="itemSeperator"></param>
        /// <returns></returns>
        public static string PrimeUIPickListKeyValue<k, v>(Dictionary<k, v> list, string KeyValueSeperator, string itemSeperator)
        {
            List<string> temp = new List<string>();
            foreach (KeyValuePair<k, v> item in list)
            {
                temp.Add(item.Key + KeyValueSeperator + item.Value);
            }

            return string.Join(itemSeperator, temp.ToArray());
        }

        /// <summary>
        /// This method creates a string of key value pairs for use with the primefaces ui PickList, using this implementations specific seperators.
        /// </summary>
        /// <typeparam name="k"></typeparam>
        /// <typeparam name="v"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string PrimeUIPickListKeyValue<k, v>(Dictionary<k, v> list)
        {
            return PrimeUIPickListKeyValue(list, ":", "|");
        }

        public static Dictionary<k, v> PrimeUIPickListDecode<k, v>(string keyValueArray, string KeyValueSeperator, string itemSeperator)
        {
            Dictionary<k, v> temp = new Dictionary<k, v>();

            foreach (string item in keyValueArray.Split(itemSeperator.ToCharArray()))
            {
                if (!String.IsNullOrWhiteSpace(item))
                {
                    k Key = (k)TypeDescriptor.GetConverter(typeof(k)).ConvertFromString(item.Split(KeyValueSeperator.ToCharArray())[0]);
                    v Value = (v)TypeDescriptor.GetConverter(typeof(v)).ConvertFromString(item.Split(KeyValueSeperator.ToCharArray())[1]);

                    temp.Add(Key, Value);
                }
            }

            return temp;
        }

        public static Dictionary<k, v> PrimeUIPickListDecode<k, v>(string keyValueArray)
        {
            return PrimeUIPickListDecode<k, v>(keyValueArray, ":", "|");
        }
    }
}