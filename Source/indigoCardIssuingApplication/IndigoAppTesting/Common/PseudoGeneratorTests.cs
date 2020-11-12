using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.Integration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Common.Tests
{
    [TestClass()]
    public class PseudoGeneratorTests
    {
        [TestMethod()]
        public void GenerateTest()
        {
            var pans = new string[1000 * 100];
            for (int i = 0; i < pans.Length; i++)
                pans[i] = "123456" + i.ToString("000000000000");



            Dictionary<string, string> unq = new Dictionary<string, string>();
            int dupCount = 0;

            for (int x = 0; x < 100; x++)
            {
                string[] pansloaded = new string[1000];

                Array.Copy(pans, x * 1000, pansloaded, 0, 1000);

                var result = PseudoGenerator.Generate(8, pansloaded, 0, 4, PseudoGenerator.PseudoType.AlphaNumeric);


                for (int i = 0; i < result.Length; i++)
                {
                    if (unq.ContainsKey(result[i]))
                    {
                        Console.WriteLine("Duplicate Found: " + result[i]);
                        dupCount++;
                    }
                    else
                        unq.Add(result[i], result[i]);
                }
            }

            Console.WriteLine("Done " + dupCount);

        }
    }
}