using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Objects;
using System.Threading.Tasks;
using System.Data.Entity;
using System.IO;
using System.Diagnostics;
using Veneka.Indigo.Integration.FileLoader.Crypto;
using Veneka.Indigo.Integration.Cryptography;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Security;
using System.Security.Cryptography;
using Veneka.Indigo.NotificationService.BLL;
using System.Threading;

namespace VenekaTesting
{
    [TestClass]
    public class MainLicenseTest
    {
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

            //BranchDAL dal = new BranchDAL("Data Source = veneka-dev; Initial Catalog = indigo_database_main_dev; integrated security = True;");
            //var lst = dal.GetCardCentreList(-1);
            //var branchLookup = lst.Where(w => w.isActive == true).FirstOrDefault();


            //Dictionary<string,string> keys = new Dictionary<string, string>();
            //Random rndOperatorKey = new Random();
            //for (int x = 0; x < 100; x++)
            //{
            //    StringBuilder randomKey = new StringBuilder();

            //    for (int i = 0; i < 4; i++)
            //        randomKey.Append(rndOperatorKey.Next(0, 9));

            //    if (keys.ContainsKey(randomKey.ToString()))
            //        Console.WriteLine("");
            //    else
            //        keys.Add(randomKey.ToString(), randomKey.ToString());
            //}


            //SecureCache sc = new SecureCache("tempTest", new TimeSpan(0, 1, 0));

            //TerminalSessionKey tsk = new TerminalSessionKey("Random", "RandomLMK");
            //sc.SetCacheItem<TerminalSessionKey>("one", tsk);

            //var tsk2 = sc.GetCachedItem<TerminalSessionKey>("one");

            //Console.WriteLine(tsk2.RandomKey);


            //CardsDAL cardsDal = new CardsDAL("Data Source=veneka-dev;Initial Catalog=indigo_database_main_dev;integrated security=True;");

            //var card = cardsDal.GetCardObject(87310, 0, -2, "test");
            

            //PGP pgp = new PGP();

            //using (var fileStream = File.Create(@"C:\veneka\pgp\Sample_enc.pgp"))
            //{
            //    using (var strm = pgp.EncryptFile(File.OpenRead(@"C:\veneka\pgp\Sample.txt"), File.ReadAllText(@"C:\veneka\pgp\bk\PublicKey.txt")))
            //    {
            //        strm.CopyTo(fileStream);
            //    }                    
            //}
                        

            //var input = File.OpenRead(@"C:\veneka\pgp\Sample_enc.pgp");
            //var key = File.ReadAllText(@"c:\veneka\pgp\bk\PrivateKey - Copy.txt");

            //var file = pgp.DecryptFile(input, key, "v3n3ka!");

            //using (var fileStream = File.Create(@"c:\veneka\pgp\Sample_decrypted.txt"))
            //{
            //    //file.Seek(0, SeekOrigin.Begin);
            //    file.CopyTo(fileStream);
                
            //}
        }

        [TestMethod]
        public void FileProcessTest()
        {
            PGP pgp = new PGP();
            var input = File.OpenRead(@"c:\veneka\pgp\B0021002_484_178.OUT.gpg");
            var key = File.ReadAllText(@"c:\veneka\pgp\PrivateKey.txt");

            var file = pgp.DecryptFile(input, key, "v3n3ka!");

            using (var fileStream = File.Create(@"c:\veneka\pgp\decrypted"))
            {
                //file.Seek(0, SeekOrigin.Begin);
                file.CopyTo(fileStream);

            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            List<CardObject> cardObjects = new List<CardObject>();

            int count = 0;
            for (int i = 0; i < 15000; i++)
            {
                count++;

                CardObject co = new CardObject();
                //(1, "", 1, "IssuerCode", "IssuerName", 1, "BranchCode" + count, "branchName",
                //                                    1, "ProductCode", "123456", 16, 1, 1, 1, 1, "", "", "", null, null, 0, 0);
                co.ProductName = "ProductName";

                cardObjects.Add(co);

                if (count >= 10)
                    count = 0;
            }
            //<product_code><dateyyyyMMdd><timehhmm>.xml

            string parameterPath = @"C:\veneka\parmTest\{isSuer_naMe}\{product_code}{daTeyyyyMMdd}{tiMehhmm}.xml";
            DateTime runDateTime = DateTime.Now;


            FilePath filePath = new FilePath();

            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            //for (int i = 0; i < 50; i++)
            //{
            var output = filePath.CreateDirectory(parameterPath, cardObjects, runDateTime);
            //}
            //sw.Stop();

            //Console.WriteLine("Elapsed: " + sw.Elapsed.TotalMilliseconds.ToString());

            foreach (var item in output)
            {
                Console.WriteLine("a");
            }



        }
    }
}
