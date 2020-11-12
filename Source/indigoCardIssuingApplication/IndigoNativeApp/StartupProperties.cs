using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndigoDesktopApp
{
    public class StartupProperties
    {
        public enum Actions
        {
            PrintCard = 0,
            PINSelect = 1
        }

        public static Actions? Action { get; set; }
        public static Uri URL { get; set; }
        public static string Token { get; set; }

        public static string Arg { get; set; }

        public static bool ExtractProperties(string argument)
        {
            
            if(argument.StartsWith("IndigoNativeApp://", StringComparison.OrdinalIgnoreCase))
            {
                Arg = argument;

                var items = argument.Substring(18).Split(',');

                Action = (Actions)int.Parse(items[0]);

                UriBuilder b;
                if (String.IsNullOrWhiteSpace(items[3]))
                {
                    b = new UriBuilder(items[1], items[2]);
                }
                else
                {
                    b = new UriBuilder(items[1], items[2], int.Parse(items[3]));
                }
                
                URL = b.Uri;                
                Token = items[4];

                return true;
            }

            return false;

        }
    }
}
