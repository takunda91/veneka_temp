using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using ZebraSdk = Zebra.Sdk;
using Microsoft.QualityTools.Testing.Fakes.Shims;
using Zebra.Sdk.Card.Containers;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra.Error.Tests
{
    [TestClass()]
    public class ZebraErrorFactoryTests
    {
        [TestMethod()]
        public void GetErrorDescription_GetErrorDescription_CurrentCulture_ForErrorNotSpecifiedInResourceFile()
        {
            // Arrange            
            ZebraErrorFactory factory = new ZebraErrorFactory();

            // Act
            var errDesc = factory.GetErrorDescription(999999999);

            // Assert
            errDesc.Should().NotBeNull(because: "Resource file must have an entry for unknown error codes.");
            errDesc.Code.Should().Be(999999999, because: "Factory must set the same error code that it was passed.");
            errDesc.Description.Should().Be("Please consult Zebra documentation or support for error code - 999999999");
            errDesc.HelpfulHint.Should().Be("Please consult Zebra documentation or support for error code - 999999999");
        }

        [TestMethod()]
        public void GetErrorDescription_GetErrorDescription_CurrentCulture_ForNoSystemErrors()
        {
            // Arrange            
            ZebraErrorFactory factory = new ZebraErrorFactory();

            // Act
            var errDesc = factory.GetErrorDescription(0);

            // Assert
            errDesc.Should().NotBeNull(because: "Resource file must have an entry for error code 0(no system errors)");
            errDesc.Code.Should().Be(0, because: "Factory must set the same error code that it was passed.");
            errDesc.Description.Should().NotBeNullOrWhiteSpace(because: "Error description in resource file must be populated");
            errDesc.HelpfulHint.Should().NotBeNullOrWhiteSpace(because: "Helpful hint in resource file must be populated");

            errDesc.Description.Should().NotBe("Please consult Zebra documentation or support for error code - 0",
                                                because: "0 must be correctly populated in resource file");
            errDesc.HelpfulHint.Should().NotBe("Please consult Zebra documentation or support for error code - 0",
                                                because: "0 must be correctly populated in resource file");
        }

        [TestMethod()]
        public void GetErrorDescription_GetErrorDescription_UnsupportedCulture_ForLidOpen()
        {
            // Arrange            
            ZebraErrorFactory factory = new ZebraErrorFactory();

            // Act
            var errDesc = factory.GetErrorDescription(7008, new System.Globalization.CultureInfo("ja-JP"));

            // Assert
            errDesc.Should().NotBeNull(because: "Resource file must provide default entry for lid open");
            errDesc.Code.Should().Be(7008, because: "Factory must set the same error code that it was passed.");
            errDesc.Description.Should().NotBeNullOrWhiteSpace(because: "Error description of default resource file should populate this field");
            errDesc.HelpfulHint.Should().NotBeNullOrWhiteSpace(because: "Helpful hint of default resource file should populate this field");
        }


        [TestMethod()]
        public void GetErrorDescription_GetErrorDescription_CurrentCulture_ForLidOpen()
        {
            // Arrange            
            ZebraErrorFactory factory = new ZebraErrorFactory();

            // Act
            var errDesc = factory.GetErrorDescription(7008);

            // Assert
            errDesc.Should().NotBeNull(because: "Resource file must have an entry for error code 7008(Lid open)");
            errDesc.Code.Should().Be(7008, because: "Factory must set the same error code that it was passed.");
            errDesc.Description.Should().NotBeNullOrWhiteSpace(because: "Error description in resource file must be populated");
            errDesc.HelpfulHint.Should().NotBeNullOrWhiteSpace(because: "Helpful hint in resource file must be populated");

            errDesc.Description.Should().NotBe("Please consult Zebra documentation or support for error code - 7008", 
                                                because: "7008 must be correctly populated in resource file");
            errDesc.HelpfulHint.Should().NotBe("Please consult Zebra documentation or support for error code - 7008",
                                                because: "7008 must be correctly populated in resource file");
        }

        [TestMethod()]
        public void GetErrorDescription_GetErrorDescription_CurrentCulture_ForInputHopperOpen()
        {
            // Arrange            
            ZebraErrorFactory factory = new ZebraErrorFactory();

            // Act
            var errDesc = factory.GetErrorDescription(7028);

            // Assert
            errDesc.Should().NotBeNull(because: "Resource file must have an entry for error code 7028(Input hopper open)");
            errDesc.Code.Should().Be(7028, because: "Factory must set the same error code that it was passed.");
            errDesc.Description.Should().NotBeNullOrWhiteSpace(because: "Error description in resource file must be populated");
            errDesc.HelpfulHint.Should().NotBeNullOrWhiteSpace(because: "Helpful hint in resource file must be populated");

            errDesc.Description.Should().NotBe("Please consult Zebra documentation or support for error code - 7028",
                                                because: "7028 must be correctly populated in resource file");
            errDesc.HelpfulHint.Should().NotBe("Please consult Zebra documentation or support for error code - 7028",
                                                because: "7028 must be correctly populated in resource file");
        }

        [TestMethod()]
        public void GetErrorDescription_GetErrorDescription_CurrentCulture_ForOutOfRibbon()
        {
            // Arrange            
            ZebraErrorFactory factory = new ZebraErrorFactory();

            // Act
            var errDesc = factory.GetErrorDescription(5001);

            // Assert
            errDesc.Should().NotBeNull(because: "Resource file must have an entry for error code 5001(out of ribbon)");
            errDesc.Code.Should().Be(5001, because: "Factory must set the same error code that it was passed.");
            errDesc.Description.Should().NotBeNullOrWhiteSpace(because: "Error description in resource file must be populated");
            errDesc.HelpfulHint.Should().NotBeNullOrWhiteSpace(because: "Helpful hint in resource file must be populated");

            errDesc.Description.Should().NotBe("Please consult Zebra documentation or support for error code - 5001",
                                                because: "5001 must be correctly populated in resource file");
            errDesc.HelpfulHint.Should().NotBe("Please consult Zebra documentation or support for error code - 5001",
                                                because: "5001 must be correctly populated in resource file");
        }
    }
}
