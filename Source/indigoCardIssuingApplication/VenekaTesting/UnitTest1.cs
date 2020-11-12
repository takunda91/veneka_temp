using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.Integration.FileLoader;
using Veneka.Indigo.Integration.FileLoader.DAL;

namespace VenekaTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            FileLoaderDAL dal = new FileLoaderDAL("Data Source=veneka-dev;Initial Catalog=indigo_database_main_dev;integrated security=True;");
            var results = dal.FetchOutstandingOrder(172, 2, -2, "SYSTEM");
        }

        [TestMethod]
        public void TestFileProcessor()
        {
            //FileProcessor processor = new FileProcessor("", "", "", )
        }
    }
}
