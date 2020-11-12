using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.UX.NativeAppAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.UX.NativeAppAPI.Tests
{
    [TestClass()]
    public class CardDataTests
    {
        [TestMethod()]
        public void CheckNullSetTest()
        {
            CardData cardData = new CardData();

            cardData.PINBlock = "abcd";

            cardData.PINBlock = "efgh";
        }
    }
}