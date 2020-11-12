using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.Integration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Common.Tests
{
    [TestClass()]
    public class LocalStorageSeqenceGeneratorTests
    {
        [TestMethod()]
        public void NextSequenceTest()
        {
            List<int> seq = new List<int>();
            List<int> expectedSeq = new List<int>();

            var dir = new System.IO.DirectoryInfo("c:\\veneka\\seq\\");
            dir.Delete(true);


            using (FileSequenceGenerator _seq = new FileSequenceGenerator(dir))
            {
                for (int i = 0; i < 1000; i++)
                {
                    seq.Add(_seq.NextSequenceNumber("test", ResetPeriod.DAILY));
                    expectedSeq.Add(i +1);
                }
            }

            Assert.IsTrue(expectedSeq.SequenceEqual(seq));

            seq = new List<int>();
            expectedSeq = new List<int>();
            using (FileSequenceGenerator _seq = new FileSequenceGenerator(dir))
            {
                for (int i = 0; i < 1000; i++)
                {
                    seq.Add(_seq.NextSequenceNumber("test", ResetPeriod.DAILY));
                    expectedSeq.Add(i + 1001);
                }
            }

            Assert.IsTrue(expectedSeq.SequenceEqual(seq));
        }
    }
}