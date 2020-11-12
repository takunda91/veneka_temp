using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.DesktopApp.Device.Printer.Zebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using FluentAssertions;
using System.Drawing;
using Zebra.Sdk.Card.Graphics;
using Microsoft.QualityTools.Testing.Fakes;
using ZebraSdk = Zebra.Sdk;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra.Tests
{
    [TestClass()]
    public class ZebraCardTextDetailTests
    {
        [TestMethod()]
        public void ZebraCardTextDetail_ConstructorValuesSetCorrectly()
        {
            // Arrange


            // Act
            var textDetail = new ZebraCardTextDetail("TextArg", 12, 14, "fontArg", 10, 123, FontStyle.Regular);

            // Assert
            textDetail.Text.Should().Be("TextArg", because: "Property must match argument passed in constructor");
            textDetail.X.Should().Be(12, because: "Property must match argument passed in constructor");
            textDetail.Y.Should().Be(14, because: "Property must match argument passed in constructor");
            textDetail.Font.Should().Be("fontArg", because: "Property must match argument passed in constructor");
            textDetail.FontSize.Should().Be(10, because: "Property must match argument passed in constructor");
            textDetail.FontColourARGB.Should().Be(123, because: "Property must match argument passed in constructor");
            textDetail.FontType.Should().Be(FontStyle.Regular, because: "Property must match argument passed in constructor");
        }

        [TestMethod()]
        public void ZebraCardTextDetail_Constructor_Text_NullArgumentException()
        {
            // Arrange
            Exception receivedException = null;

            // Act
            try
            {
                var textDetail = new ZebraCardTextDetail(null, 12, 14, "fontArg", 10, 123, FontStyle.Regular);
            }
            catch(Exception ex)
            {
                receivedException = ex;
            }

            // Assert
            receivedException.Should().NotBeNull(because: "ArgumentNullException must be thrown on null text argument");
            receivedException.Should().BeOfType(typeof(ArgumentNullException));
            ((ArgumentNullException)receivedException).ParamName.Should().Be("text");
        }

        [TestMethod()]
        public void ZebraCardTextDetail_Constructor_Text_EmptyStringArgumentException()
        {
            // Arrange
            Exception receivedException = null;

            // Act
            try
            {
                var textDetail = new ZebraCardTextDetail("", 12, 14, "fontArg", 10, 123, FontStyle.Regular);
            }
            catch (Exception ex)
            {
                receivedException = ex;
            }

            // Assert
            receivedException.Should().NotBeNull(because: "ArgumentNullException must be thrown on null text argument");
            receivedException.Should().BeOfType(typeof(ArgumentNullException));
            ((ArgumentNullException)receivedException).ParamName.Should().Be("text");
        }

        [TestMethod()]
        public void ZebraCardTextDetail_Constructor_Font_NullArgumentException()
        {
            // Arrange
            Exception receivedException = null;

            // Act
            try
            {
                var textDetail = new ZebraCardTextDetail("TextArg", 12, 14, null, 10, 123, FontStyle.Regular);
            }
            catch (Exception ex)
            {
                receivedException = ex;
            }

            // Assert
            receivedException.Should().NotBeNull(because: "ArgumentNullException must be thrown on null font argument");
            receivedException.Should().BeOfType(typeof(ArgumentNullException));
            ((ArgumentNullException)receivedException).ParamName.Should().Be("font");
        }

        [TestMethod()]
        public void ZebraCardTextDetail_Constructor_Font_EmptyStringArgumentException()
        {
            // Arrange
            Exception receivedException = null;

            // Act
            try
            {
                var textDetail = new ZebraCardTextDetail("TextArg", 12, 14, "", 10, 123, FontStyle.Regular);
            }
            catch (Exception ex)
            {
                receivedException = ex;
            }

            // Assert
            receivedException.Should().NotBeNull(because: "ArgumentNullException must be thrown on null font argument");
            receivedException.Should().BeOfType(typeof(ArgumentNullException));
            ((ArgumentNullException)receivedException).ParamName.Should().Be("font");
        }
    }
}