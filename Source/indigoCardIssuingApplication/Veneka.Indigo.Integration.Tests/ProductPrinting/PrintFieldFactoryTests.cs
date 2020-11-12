using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.Integration.ProductPrinting.Tests
{
    [TestClass]
    public class PrintFieldFactoryTests
    {
        #region Tests for CreatePrintField(int printFieldTypeId)
        [TestMethod]
        public void CreatePrintField_UsingPrintFieldTypeId0_ShouldReturnPrintStringField()
        {
            
            // Arrange
            int printFieldTypeId = 0;

            // Act
            var resultField = PrintFieldFactory.CreatePrintField(printFieldTypeId);

            // Assert
            resultField.Should().NotBeNull(because: "Creating field based on PrintFieldTypeId=0 must return a new object from the factory.");
            resultField.Should().BeOfType(typeof(PrintStringField), because: "Creating field with PrintFieldTypeId=0 must return an object of type PrintStringField");
        }

        [TestMethod]
        public void CreatePrintField_UsingPrintFieldTypeId1_ShouldReturnPrintImageField()
        {
            // Arrange
            int printFieldTypeId = 1;

            // Act
            var resultField = PrintFieldFactory.CreatePrintField(printFieldTypeId);

            // Assert
            resultField.Should().NotBeNull(because: "Creating field based on PrintFieldTypeId must return a new object from the factory.");
            resultField.Should().BeOfType(typeof(PrintImageField), because: "Creating field with PrintFieldTypeId=1 must return an object of type PrintImageField");
        }

        [TestMethod]
        public void CreatePrintField_UsingPrintFieldTypeIdNot0or1_ShouldThrowAnException()
        {
            // Arrange
            int printFieldTypeId = 2;

            // Act
            Exception thrownException = null;
            try
            {
                var resultField = PrintFieldFactory.CreatePrintField(printFieldTypeId);
            }
            catch(Exception ex)
            {
                thrownException = ex;
            }

            // Assert
            thrownException.Should().NotBeNull(because: "Passing in a PrintFieldTypeId that is not 1 or 0 must throw an exception");
        }

        [TestMethod]
        public void CreatePrintField_UsingPrintFieldTypeIdNot0or1_MustThrowArgumentException()
        {
            // Arrange
            int printFieldTypeId = 2;

            // Act
            Exception thrownException = null;
            try
            {
                var resultField = PrintFieldFactory.CreatePrintField(printFieldTypeId);
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }

            // Assert
            thrownException.Should().BeOfType(typeof(ArgumentException), because: "Passing in a PrintFieldTypeID that is not 1 or 0 must throw an exception of type ArgumentException");
            thrownException.Message.Should().Be("Value of " + printFieldTypeId + " not supported.\r\nParameter name: printFieldTypeId", because: "Exception message must indicate what is wrong with the argument");
        }
        #endregion

        #region Tests for CreatePrintField with Short Parameters
        [TestMethod]
        public void CreatePrintField_UsingShortParameterMethod_ShouldReturnPrintStringFieldWithCorrectPropertiesSet()
        {

            // Arrange
            int printFieldTypeId = 0;
            PrintStringField printField = new PrintStringField()
            {
                Deleted = false,
                Editable = false,
                FieldValue = null,
                Font = "Font",
                FontColourRGB = 0,
                FontSize = 10,
                Height = 30,
                Label = "Label",
                MappedName = "Mapped Name",
                MaxSize = 50,
                Name = "Name",
                Printable = false,
                PrintSide = 0,
                ProductPrintFieldId = 0,
                Value = null,
                Width = 100,
                X = 87,
                Y = 243
            };

            // Act
            var resultField = PrintFieldFactory.CreatePrintField(printFieldTypeId, printField.Name, printField.X, printField.Y,
                                                                printField.Width, printField.Height, printField.Font, 
                                                                printField.FontSize, printField.MappedName, 
                                                                printField.Label, printField.MaxSize);

            // Assert
            resultField.Should().NotBeNull(because: "Creating field based on PrintFieldTypeId=0 must return a new object from the factory.");
            resultField.Should().BeOfType(typeof(PrintStringField), because: "Creating field with PrintFieldTypeId=0 must return an object of type PrintStringField");
            resultField.Should().BeEquivalentTo(printField, because: "Parameters set via method must set correct properties on the object");
        }

        [TestMethod]
        public void CreatePrintField_UsingShortParameterMethod_ShouldReturnPrintImageFieldWithCorrectPropertiesSet()
        {

            // Arrange
            int printFieldTypeId = 1;
            PrintImageField printField = new PrintImageField()
            {
                Deleted = false,
                Editable = false,                
                Font = "Font",
                FontColourRGB = 0,
                FontSize = 10,
                Height = 30,
                Label = "Label",
                MappedName = "Mapped Name",
                MaxSize = 50,
                Name = "Name",
                Printable = false,
                PrintSide = 0,
                ProductPrintFieldId = 0,
                Value = null,
                Width = 100,
                X = 87,
                Y = 243                
            };

            // Act
            var resultField = PrintFieldFactory.CreatePrintField(printFieldTypeId, printField.Name, printField.X, printField.Y,
                                                                printField.Width, printField.Height, printField.Font,
                                                                printField.FontSize, printField.MappedName,
                                                                printField.Label, printField.MaxSize);

            // Assert
            resultField.Should().NotBeNull(because: "Creating field based on PrintFieldTypeId=1 must return a new object from the factory.");
            resultField.Should().BeOfType(typeof(PrintImageField), because: "Creating field with PrintFieldTypeId=1 must return an object of type PrintImageField");
            resultField.Should().BeEquivalentTo(printField, because: "Parameters set via method must set correct properties on the object");
        }

        [TestMethod]
        public void CreatePrintField_UsingShortParameterMethodAndIncorrectPrintFieldTypeId_ShouldThrowArgumentException()
        {

            // Arrange
            int printFieldTypeId = 2;
            PrintImageField printField = new PrintImageField()
            {
                Deleted = false,
                Editable = false,
                Font = "Font",
                FontColourRGB = 0,
                FontSize = 10,
                Height = 30,
                Label = "Label",
                MappedName = "Mapped Name",
                MaxSize = 50,
                Name = "Name",
                Printable = false,
                PrintSide = 0,
                ProductPrintFieldId = 0,
                Value = null,
                Width = 100,
                X = 87,
                Y = 243
            };

            // Act
            Exception thrownException = null;
            try
            {
                var resultField = PrintFieldFactory.CreatePrintField(printFieldTypeId, printField.Name, printField.X, printField.Y,
                                                                    printField.Width, printField.Height, printField.Font,
                                                                    printField.FontSize, printField.MappedName,
                                                                    printField.Label, printField.MaxSize);
            }
            catch(Exception ex)
            {
                thrownException = ex;
            }

            // Assert
            thrownException.Should().NotBeNull(because: "An exception must be thrown if the incorrect printFieldTypeId is passed to method");
            thrownException.Should().BeOfType(typeof(ArgumentException), because: "Incorrect printFieldTypeId must throw an exception of type ArgumentException");
            thrownException.Message.Should().Be("Value of " + printFieldTypeId + " not supported.\r\nParameter name: printFieldTypeId", because: "Exception message must indicate what is wrong with the argument");
        }    
        #endregion

        #region Tests for CreatePrintField with Long Parameters
        [TestMethod]
        public void CreatePrintField_UsingLongParameterMethod_ShouldReturnPrintStringFieldWithCorrectPropertiesSet()
        {
            // Arrange
            int printFieldTypeId = 0;
            PrintStringField printField = new PrintStringField()
            {
                Deleted = true,
                Editable = true,
                FieldValue = null,
                Font = "Font",
                FontColourRGB = 123,
                FontSize = 10,
                Height = 30,
                Label = "Label",
                MappedName = "Mapped Name",
                MaxSize = 50,
                Name = "Name",
                Printable = true,
                PrintSide = 0,
                ProductPrintFieldId = 7,
                Value = "Value",
                Width = 100,
                X = 87,
                Y = 243
            };

            // Act
            var resultField = PrintFieldFactory.CreatePrintField(printFieldTypeId, printField.ProductPrintFieldId, printField.Name, 
                                                                printField.X, printField.Y,
                                                                printField.Width, printField.Height, printField.Font,
                                                                printField.FontSize, printField.FontColourRGB, printField.MappedName,
                                                                printField.Label, printField.MaxSize, 
                                                                printField.Editable, printField.Deleted,
                                                                printField.Printable,printField.PrintSide, System.Text.Encoding.UTF8.GetBytes(printField.Value));

            // Assert
            resultField.Should().NotBeNull(because: "Creating field based on PrintFieldTypeId=0 must return a new object from the factory.");
            resultField.Should().BeOfType(typeof(PrintStringField), because: "Creating field with PrintFieldTypeId=0 must return an object of type PrintStringField");
            resultField.Should().BeEquivalentTo(printField, because: "Parameters set via method must set correct properties on the object");
        }

        [TestMethod]
        public void CreatePrintStringField_UsingLongParameterMethod_ShouldReturnPrintStringFieldWithCorrectPropertiesSet()
        {
            // Arrange            
            PrintStringField printField = new PrintStringField()
            {
                Deleted = true,
                Editable = true,
                FieldValue = null,
                Font = "Font",
                FontColourRGB = 123,
                FontSize = 10,
                Height = 30,
                Label = "Label",
                MappedName = "Mapped Name",
                MaxSize = 50,
                Name = "Name",
                Printable = true,
                PrintSide = 0,
                ProductPrintFieldId = 7,
                Value = "Value",
                Width = 100,
                X = 87,
                Y = 243
            };

            // Act
            var resultField = PrintFieldFactory.CreatePrintStringField(printField.ProductPrintFieldId, printField.Name,
                                                                printField.X, printField.Y,
                                                                printField.Width, printField.Height, printField.Font,
                                                                printField.FontSize, printField.FontColourRGB, printField.MappedName,
                                                                printField.Label, printField.MaxSize,
                                                                printField.Editable, printField.Deleted,
                                                                printField.Printable, printField.PrintSide, printField.Value);

            // Assert
            resultField.Should().NotBeNull(because: "Creating field based on PrintFieldTypeId=0 must return a new object from the factory.");
            resultField.Should().BeOfType(typeof(PrintStringField), because: "Creating field with PrintFieldTypeId=0 must return an object of type PrintStringField");
            resultField.Should().BeEquivalentTo(printField, because: "Parameters set via method must set correct properties on the object");
        }

        [TestMethod]
        public void CreatePrintField_UsingLongParameterMethod_ShouldReturnPrintImageFieldWithCorrectPropertiesSet()
        {
            // Arrange
            int printFieldTypeId = 1;
            PrintImageField printField = new PrintImageField()
            {
                Deleted = true,
                Editable = true,
                Font = "Font",
                FontColourRGB = 123,
                FontSize = 10,
                Height = 30,
                Label = "Label",
                MappedName = "Mapped Name",
                MaxSize = 50,
                Name = "Name",
                Printable = true,
                PrintSide = 0,
                ProductPrintFieldId = 7,
                Width = 100,
                X = 87,
                Y = 243,
                Value = new byte[] { 0x10, 0x23, 0x45 }
            };

            // Act
            var resultField = PrintFieldFactory.CreatePrintField(printFieldTypeId, printField.ProductPrintFieldId, printField.Name,
                                                                printField.X, printField.Y,
                                                                printField.Width, printField.Height, printField.Font,
                                                                printField.FontSize, printField.FontColourRGB, printField.MappedName,
                                                                printField.Label, printField.MaxSize,
                                                                printField.Editable, printField.Deleted,
                                                                printField.Printable, printField.PrintSide, printField.Value);

            // Assert
            resultField.Should().NotBeNull(because: "Creating field based on PrintFieldTypeId=1 must return a new object from the factory.");
            resultField.Should().BeOfType(typeof(PrintImageField), because: "Creating field with PrintFieldTypeId=1 must return an object of type PrintImageField");
            resultField.Should().BeEquivalentTo(printField, because: "Parameters set via method must set correct properties on the object");
        }

        [TestMethod]
        public void CreatePrintField_UsingLongParameterMethodAndIncorrectPrintFieldTypeId_ShouldThrowArgumentException()
        {

            // Arrange
            int printFieldTypeId = 2;
            PrintImageField printField = new PrintImageField()
            {
                Deleted = false,
                Editable = false,
                Font = "Font",
                FontColourRGB = 0,
                FontSize = 10,
                Height = 30,
                Label = "Label",
                MappedName = "Mapped Name",
                MaxSize = 50,
                Name = "Name",
                Printable = false,
                PrintSide = 0,
                ProductPrintFieldId = 0,
                Value = null,
                Width = 100,
                X = 87,
                Y = 243
            };
          
            // Act
            Exception thrownException = null;
            try
            {
                var resultField = PrintFieldFactory.CreatePrintField(printFieldTypeId, printField.ProductPrintFieldId, printField.Name,
                                                                printField.X, printField.Y,
                                                                printField.Width, printField.Height, printField.Font,
                                                                printField.FontSize, printField.FontColourRGB, printField.MappedName,
                                                                printField.Label, printField.MaxSize,
                                                                printField.Editable, printField.Deleted,
                                                                printField.Printable, printField.PrintSide, printField.Value);
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }

            // Assert
            thrownException.Should().NotBeNull(because: "An exception must be thrown if the incorrect printFieldTypeId is passed to method");
            thrownException.Should().BeOfType(typeof(ArgumentException), because: "Incorrect printFieldTypeId must throw an exception of type ArgumentException");
            thrownException.Message.Should().Be("Value of " + printFieldTypeId + " not supported.\r\nParameter name: printFieldTypeId", because: "Exception message must indicate what is wrong with the argument");
        }
        #endregion
    }
}
