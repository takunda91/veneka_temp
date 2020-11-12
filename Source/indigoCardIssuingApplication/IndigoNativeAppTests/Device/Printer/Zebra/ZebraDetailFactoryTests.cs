using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.DesktopApp.Device.Printer.Zebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using FluentAssertions;
using Veneka.Indigo.Integration.ProductPrinting;
using System.Drawing;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra.Tests
{
    [TestClass()]
    public class ZebraDetailFactoryTests
    {
        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithSingleProductField_Success()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim") }
            };

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelText.Length.Should().Be(1, because: "Single front side text field passed in");
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithEmptyProductFieldArray_ThrowsArgumentException()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();
            ProductField[] productFields = new ProductField[0];

            // Act
            Exception exception = null;

            try
            {
                var simPrintDetails = zebraDetailFactory.Populate(productFields);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            exception.Should().NotBeNull(because: "An ArgumentNullException must be thrown if an empty array is passed to Populate(). Calling code should make sure there is something to populate.");
            exception.Should().BeOfType(typeof(ArgumentNullException), because: "The populate fields argument is null or empty. Must return correct expection type to indicate this");
            ((ArgumentNullException)exception).ParamName.Should().Be("productFields", because: "This is the argument that should have thrown the ArgumentNullException");
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithNullProductFieldArray_ThrowsArgumentException()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();
            ProductField[] productFields = null;

            // Act
            Exception exception = null;

            try
            {
                var simPrintDetails = zebraDetailFactory.Populate(productFields);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            exception.Should().NotBeNull(because: "An ArgumentNullException must be thrown if a null array is passed to Populate(). Calling code should make sure there is something to populate.");
            exception.Should().BeOfType(typeof(ArgumentNullException), because: "The populate fields argument is null or empty. Must return correct expection type to indicate this");
            ((ArgumentNullException)exception).ParamName.Should().Be("productFields", because: "This is the argument that should have thrown the ArgumentNullException");
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithDeletedProductField_ThrowsArgumentException()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();
            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = true, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim") }
            };

            // Act
            Exception exception = null;

            try
            {
                var simPrintDetails = zebraDetailFactory.Populate(productFields);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            exception.Should().NotBeNull(because: "An ArgumentNullException must be thrown if the array passed to Populate() does not have any valid product fields to print.");
            exception.Should().BeOfType(typeof(ArgumentNullException), because: "The populate fields array does not have any valid product fields. In this case the field is set to deleted");
            ((ArgumentNullException)exception).ParamName.Should().Be("productFields", because: "This is the argument that should have thrown the ArgumentNullException");
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithNonPrintableProductField_ThrowsArgumentException()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();
            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = false, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim") }
            };

            // Act
            Exception exception = null;

            try
            {
                var simPrintDetails = zebraDetailFactory.Populate(productFields);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            exception.Should().NotBeNull(because: "An ArgumentNullException must be thrown if the array passed to Populate() does not have any valid product fields to print.");
            exception.Should().BeOfType(typeof(ArgumentNullException), because: "The populate fields array does not have any valid product fields. In this case the field is set to not be printable");
            ((ArgumentNullException)exception).ParamName.Should().Be("productFields", because: "This is the argument that should have thrown the ArgumentNullException");
        }

        #region Testing Front Text
        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithFrontTextFields_OnlyHaveFrontTextFieldsPopulated()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelText.Length.Should().Be(2, because: "Two front side text field passed to Populate(). Must set two FrontPanelText objects.");
            simPrintDetails.BackPanelText.Length.Should().Be(0, because: "No back side text fields passed to Populate()");
            simPrintDetails.FrontPanelImages.Length.Should().Be(0, because: "No image fields passed to Populate()");
            simPrintDetails.BackPanelImages.Length.Should().Be(0, because: "No image fields passed to Populate()");
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithTwoFrontTextFields_FirstFrontPanelFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            var firstField = productFields[0];
            var firstTextField = new ZebraCardTextDetail(firstField.ValueToString(), firstField.X, firstField.Y, firstField.Font,
                                                            firstField.FontSize, firstField.FontColourRGB, FontStyle.Regular);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelText[0].Should().BeEquivalentTo(firstTextField);

        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithTwoFrontTextFields_SecondFrontPanelFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            var secondField = productFields[1];
            var secondTextField = new ZebraCardTextDetail(secondField.ValueToString(), secondField.X, secondField.Y, secondField.Font,
                                                            secondField.FontSize, secondField.FontColourRGB, FontStyle.Regular);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelText[1].Should().BeEquivalentTo(secondTextField);
        }
        #endregion

        #region Testing Back Text
        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithBackTextFields_OnlyHaveBackTextFieldsPopulated()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelText.Length.Should().Be(0, because: "No front side text fields passed to Populate()");
            simPrintDetails.BackPanelText.Length.Should().Be(2, because: "Two back side text field passed to Populate(). Must set two BackPanelText objects.");
            simPrintDetails.FrontPanelImages.Length.Should().Be(0, because: "No image fields passed to Populate()");
            simPrintDetails.BackPanelImages.Length.Should().Be(0, because: "No image fields passed to Populate()");
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithTwoBackTextFields_FirstBackPanelFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 1,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 1,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            var firstField = productFields[0];
            var firstTextField = new ZebraCardTextDetail(firstField.ValueToString(), firstField.X, firstField.Y, firstField.Font,
                                                            firstField.FontSize, firstField.FontColourRGB, FontStyle.Regular);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.BackPanelText[0].Should().BeEquivalentTo(firstTextField);

        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithTwoBackTextFields_SecondBackPanelFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 1,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 1,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            var secondField = productFields[1];
            var secondTextField = new ZebraCardTextDetail(secondField.ValueToString(), secondField.X, secondField.Y, secondField.Font,
                                                            secondField.FontSize, secondField.FontColourRGB, FontStyle.Regular);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.BackPanelText[1].Should().BeEquivalentTo(secondTextField);
        }
        #endregion

        #region Testing Front Image
        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithFrontImageFields_OnlyHaveFrontImageFieldsPopulated()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelText.Length.Should().Be(0, because: "No text fields passed to Populate()");
            simPrintDetails.BackPanelText.Length.Should().Be(0, because: "No text fields passed to Populate()");
            simPrintDetails.FrontPanelImages.Length.Should().Be(2, because: "Two front side image field passed to Populate(). Must set two FrontPanelImages objects.");
            simPrintDetails.BackPanelImages.Length.Should().Be(0, because: "No back side image fields passed to Populate()");
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithTwoFrontImageFields_FirstFrontPanelFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            var firstField = productFields[0];
            var firstImageField = new ZebraCardImageDetail(firstField.Value, firstField.X, firstField.Y, firstField.Width, firstField.Height);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelImages[0].Should().BeEquivalentTo(firstImageField);

        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithTwoFrontImageFields_SecondFrontPanelFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            var secondField = productFields[1];
            var secondImageField = new ZebraCardImageDetail(secondField.Value, secondField.X, secondField.Y, secondField.Width, secondField.Height);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelImages[1].Should().BeEquivalentTo(secondImageField);
        }
        #endregion

        #region Testing Back Image
        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithBackImageFields_OnlyHaveBackImageFieldsPopulated()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelText.Length.Should().Be(0, because: "No text fields passed to Populate()");
            simPrintDetails.BackPanelText.Length.Should().Be(0, because: "No text fields passed to Populate()");
            simPrintDetails.FrontPanelImages.Length.Should().Be(0, because: "No front side image fields passed to Populate()");
            simPrintDetails.BackPanelImages.Length.Should().Be(2, because: "Two back side image field passed to Populate(). Must set two BackPanelImages objects.");
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithTwoBackImageFields_FirstBackPanelFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            var firstField = productFields[0];
            var firstImageField = new ZebraCardImageDetail(firstField.Value, firstField.X, firstField.Y, firstField.Width, firstField.Height);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.BackPanelImages[0].Should().BeEquivalentTo(firstImageField);

        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithTwoBackImageFields_SecondBackPanelFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") }
            };

            var secondField = productFields[1];
            var secondImageField = new ZebraCardImageDetail(secondField.Value, secondField.X, secondField.Y, secondField.Width, secondField.Height);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.BackPanelImages[1].Should().BeEquivalentTo(secondImageField);
        }
        #endregion

        #region Testing Mixed Front Fields
        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithFrontMixedFields_OnlyHaveFrontFieldsPopulated()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 43, Y = 66, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 3") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 5, Y = 9, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 4") }
            };

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelText.Length.Should().Be(2, because: "Two front side text field passed to Populate(). Must set two FrontPanelText objects.");
            simPrintDetails.FrontPanelImages.Length.Should().Be(2, because: "Two front side image field passed to Populate(). Must set two FrontPanelImages objects.");
            simPrintDetails.BackPanelText.Length.Should().Be(0, because: "No back fields passed to Populate()");
            simPrintDetails.BackPanelImages.Length.Should().Be(0, because: "No back fields passed to Populate()");
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithFrontMixedFields_FrontPanelTextFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 43, Y = 66, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 3") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 5, Y = 9, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 4") }
            };

            var firstField = productFields[0];
            var firstTextField = new ZebraCardTextDetail(firstField.ValueToString(), firstField.X, firstField.Y, firstField.Font,
                                                            firstField.FontSize, firstField.FontColourRGB, FontStyle.Regular);
            var secondField = productFields[1];
            var secondTextField = new ZebraCardTextDetail(secondField.ValueToString(), secondField.X, secondField.Y, secondField.Font,
                                                             secondField.FontSize, secondField.FontColourRGB, FontStyle.Regular);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelText[0].Should().BeEquivalentTo(firstTextField);
            simPrintDetails.FrontPanelText[1].Should().BeEquivalentTo(secondTextField);
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithFrontMixedFields_FrontPanelImageFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 43, Y = 66, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 3") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 5, Y = 9, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 4") }
            };

            var firstField = productFields[2];
            var firstImageField = new ZebraCardImageDetail(firstField.Value, firstField.X, firstField.Y, firstField.Width, firstField.Height);

            var secondField = productFields[3];
            var secondImageField = new ZebraCardImageDetail(secondField.Value, secondField.X, secondField.Y, secondField.Width, secondField.Height);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelImages[0].Should().BeEquivalentTo(firstImageField);
            simPrintDetails.FrontPanelImages[1].Should().BeEquivalentTo(secondImageField);
        }
        #endregion

        #region Testing Mixed Back Fields
        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithBackMixedFields_OnlyHaveBackFieldsPopulated()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 43, Y = 66, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 3") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 5, Y = 9, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 4") }
            };

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.BackPanelText.Length.Should().Be(2, because: "Two back side text field passed to Populate(). Must set two BackPanelText objects.");
            simPrintDetails.BackPanelImages.Length.Should().Be(2, because: "Two back side image field passed to Populate(). Must set two BackPanelImages objects.");
            simPrintDetails.FrontPanelText.Length.Should().Be(0, because: "No front fields passed to Populate()");
            simPrintDetails.FrontPanelImages.Length.Should().Be(0, because: "No front fields passed to Populate()");
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithBackMixedFields_BackPanelTextFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 43, Y = 66, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 3") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 5, Y = 9, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 4") }
            };

            var firstField = productFields[0];
            var firstTextField = new ZebraCardTextDetail(firstField.ValueToString(), firstField.X, firstField.Y, firstField.Font,
                                                            firstField.FontSize, firstField.FontColourRGB, FontStyle.Regular);
            var secondField = productFields[1];
            var secondTextField = new ZebraCardTextDetail(secondField.ValueToString(), secondField.X, secondField.Y, secondField.Font,
                                                             secondField.FontSize, secondField.FontColourRGB, FontStyle.Regular);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.BackPanelText[0].Should().BeEquivalentTo(firstTextField);
            simPrintDetails.BackPanelText[1].Should().BeEquivalentTo(secondTextField);
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithBackMixedFields_BackPanelImageFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 43, Y = 66, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 3") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 5, Y = 9, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 4") }
            };

            var firstField = productFields[2];
            var firstImageField = new ZebraCardImageDetail(firstField.Value, firstField.X, firstField.Y, firstField.Width, firstField.Height);

            var secondField = productFields[3];
            var secondImageField = new ZebraCardImageDetail(secondField.Value, secondField.X, secondField.Y, secondField.Width, secondField.Height);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.BackPanelImages[0].Should().BeEquivalentTo(firstImageField);
            simPrintDetails.BackPanelImages[1].Should().BeEquivalentTo(secondImageField);
        }
        #endregion

        #region Testing Mixed Fields
        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithMixedFields_CheckFieldsAllocatedCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 43, Y = 66, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 3") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 5, Y = 9, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 4") }
            };

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.BackPanelText.Length.Should().Be(1);
            simPrintDetails.BackPanelImages.Length.Should().Be(1);
            simPrintDetails.FrontPanelText.Length.Should().Be(1);
            simPrintDetails.FrontPanelImages.Length.Should().Be(1);
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithMixedFields_FrontPanelFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 43, Y = 66, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 3") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 5, Y = 9, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 4") }
            };

            var firstField = productFields[0];
            var firstTextField = new ZebraCardTextDetail(firstField.ValueToString(), firstField.X, firstField.Y, firstField.Font,
                                                            firstField.FontSize, firstField.FontColourRGB, FontStyle.Regular);
            var secondField = productFields[2];
            var secondTextField = new ZebraCardImageDetail(secondField.Value, secondField.X, secondField.Y, secondField.Width, secondField.Height);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.FrontPanelText[0].Should().BeEquivalentTo(firstTextField);
            simPrintDetails.FrontPanelImages[0].Should().BeEquivalentTo(secondTextField);
        }

        [TestMethod()]
        public void ZebraDetailFactory_PopulateWithMixedFields_BackPanelFieldSetCorrectly()
        {
            // Arrange
            var zebraDetailFactory = new ZebraDetailFactory();

            ProductField[] productFields = new ProductField[]
            {
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "consolas", PrintSide = 0, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Black.ToArgb(), FontSize = 10, X = 15, Y = 30, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 1") },
                new ProductField {Deleted = false, Editable = false, Printable = true, Font = "arial", PrintSide = 1, ProductPrintFieldTypeId = 0,
                    FontColourRGB = Color.Red.ToArgb(), FontSize = 12, X = 20, Y = 52, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 2") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 0, Width = 100, Height = 200, ProductPrintFieldTypeId = 1,
                    X = 43, Y = 66, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 3") },
                new ProductField {Deleted = false, Editable = false, Printable = true, PrintSide = 1, Width = 430, Height = 789, ProductPrintFieldTypeId = 1,
                    X = 5, Y = 9, Value = Encoding.UTF8.GetBytes("Hello From Print Sim field 4") }
            };

            var firstField = productFields[1];
            var firstTextField = new ZebraCardTextDetail(firstField.ValueToString(), firstField.X, firstField.Y, firstField.Font,
                                                            firstField.FontSize, firstField.FontColourRGB, FontStyle.Regular);
            var secondField = productFields[3];
            var secondTextField = new ZebraCardImageDetail(secondField.Value, secondField.X, secondField.Y, secondField.Width, secondField.Height);

            // Act
            var simPrintDetails = zebraDetailFactory.Populate(productFields);

            // Assert
            simPrintDetails.BackPanelText[0].Should().BeEquivalentTo(firstTextField);
            simPrintDetails.BackPanelImages[0].Should().BeEquivalentTo(secondTextField);
        }
        #endregion
    }
}