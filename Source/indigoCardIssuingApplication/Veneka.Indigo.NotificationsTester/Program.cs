using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Notifications.NotificationChannels;

namespace Veneka.Indigo.NotificationsTester
{
    class Program
    {
        static void Main(string[] args)
        {
           

            SMSController new_sms = new SMSController("+263772222696","ABR","Hi");
            int response_code;
            var send_sms = new_sms.sendSMS(out response_code);
            Console.WriteLine("Response code is " + response_code.ToString());
            Console.ReadLine();
        }
    }
}
