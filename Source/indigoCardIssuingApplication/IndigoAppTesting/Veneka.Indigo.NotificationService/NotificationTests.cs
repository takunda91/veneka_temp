using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.NotificationService.BLL;
using System.Threading;

namespace IndigoAppTesting.Veneka.Indigo.NotificationService
{
    [TestClass]
    public class NotificationTests
    {
        [Ignore]
        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine("Starting");
            using (NotificationsController notif = new NotificationsController(@"C:\veneka\indigo_main_dev\", "Data Source=veneka-dev;Initial Catalog=indigo_database_main_dev;integrated security=True;"))
            {
                notif.Start();
                Thread.Sleep(60000);
                notif.Stop();
            }

            //Thread.Sleep(5000);

            //using (NotificationsController notif = new NotificationsController(@"C:\veneka\indigo_main_dev\", "Data Source=veneka-dev;Initial Catalog=indigo_database_main_dev;integrated security=True;"))
            //{
            //    notif.Start();
            //    Thread.Sleep(30000);
            //    notif.Stop();
            //}
        }
    }
}
