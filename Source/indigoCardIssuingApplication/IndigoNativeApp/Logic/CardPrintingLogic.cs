using IndigoDesktopApp;
using IndigoDesktopApp.Device.Printer;
using IndigoDesktopApp.NativeAppAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using Veneka.Indigo.DesktopApp.Device.Printer;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.Security;
using Veneka.Indigo.UX.NativeAppAPI;

namespace Veneka.Indigo.DesktopApp.Logic
{
    public class CardPrintingLogic
    {
        private readonly ICardPrinting _cardPrinting;
        private readonly IPrinter _printer;

        public event UiUpdateEventHandler UiUpdate;

        public CardPrintingLogic(ICardPrinting cardPrinting, IPrinter printer)
        {
            _cardPrinting = cardPrinting;
            _printer = printer;
        }

        protected virtual void OnUiUpdate(string message, bool isError, bool append, EventArgs e)
        {
            UiUpdate?.Invoke(this, message, isError, append, e);
        }

        private void DeviceNotifcation(object sender, string message, bool isCritical, EventArgs e)
        {
            UiUpdate?.Invoke(sender, message, isCritical, true, e);
        }

        public void DoPrintJob()
        {
            try
            {
                Token token = null;

                bool complete = false;
                bool success = true;
                string additionalInfo = String.Empty;
                IPrintJob printJob = null;
                CardData cardData = null;

                // Go through steps proided the process hasnt completed or a step was not sucessful
                for (int step = 0; complete == false && success == true; step++)
                {
                    switch (step)
                    {
                        case 0: // Connect to printer and do any setup
                            success = PrinterConnectAndSetup(out token, out additionalInfo);
                            break;
                        case 1: // Get the print Job and settings            
                            success = GetPrintJob(token, out printJob, out additionalInfo);
                            break;
                        case 2: // Set Printer settings
                            success = SetPrinterSetting(printJob, out additionalInfo);
                            break;
                        case 3: // Start physical printing
                            success = StartPhysicalPrint(printJob, out cardData, out additionalInfo);
                            complete = true;
                            break;
                        default: complete = true; break;
                    }
                }

                if (complete && success)
                {
                  

                    // Update Indigo
                    _cardPrinting.PrintingComplete(token, cardData);
                    Dictionary<string, string> _printerInfo = _printer.GetPrinterInfo();
                    string cardscount = "-1";
                    _printerInfo.TryGetValue("Total Cards Printed", out  cardscount);
                    if (!string.IsNullOrEmpty(cardscount))
                    {
                        PrinterInfo printerInfo = new PrinterInfo
                        {
                            TotalPrints = int.Parse(cardscount),
                            SerialNo = _printer.DeviceId
                        };
                        // Send Analytics
                        _cardPrinting.PrinterAuditDetails(token, printerInfo);
                    }
                   
                    OnUiUpdate(CardPrintingLogicResource.PrintingDoneUi, false, true, EventArgs.Empty);
                }
                else
                {
                    OnUiUpdate(CardPrintingLogicResource.PrintingFailedUi+additionalInfo, false, true, EventArgs.Empty);
                    _cardPrinting.PrintFailed(token, additionalInfo);
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException enfex)
            {
                OnUiUpdate(enfex.Message, false, true, EventArgs.Empty);
                //Console.WriteLine(enfex);
            }
            catch
            {
                throw;
                //OnUiUpdate(String.Format("{0}{1}{2}{3}{4}", _pinPad.DllPath, Environment.NewLine,ex.Message, Environment.NewLine, ex.ToString()), EventArgs.Empty);
                //Console.WriteLine(ex);
            }
            finally
            {
                _printer.Disconnect();
                _printer.OnDeviceNotifcation -= DeviceNotifcation;
                _printer.Dispose();
            }
        }

        /// <summary>
        /// Connects to printer and does any necessary setup
        /// </summary>        
        /// <param name="token">Populated token to be used for web service calls</param>
        /// <param name="additionalInfo"></param>
        /// <returns>true if successful</returns>
        public bool PrinterConnectAndSetup(out Token token, out string additionalInfo)
        {
            OnUiUpdate(CardPrintingLogicResource.ConnectToPrinterUi, false, true, EventArgs.Empty);

            token = null;

            if (_printer.Connect() == PrinterCodes.Success)
            {
                // subscribe to printer notification events                                
                _printer.OnDeviceNotifcation += DeviceNotifcation;

                token = new Token
                {
                    DeviceID = _printer.DeviceId,
                    Session = StartupProperties.Token,
                    Workstation = Environment.MachineName
                };

                additionalInfo = CardPrintingLogicResource.ConnectToPrinterSuccessUi;
                return true;
            }

            additionalInfo = CardPrintingLogicResource.ConnectToPrinterFailedUi;
            return false;
        }

        public bool GetPrintJob(Token token, out IPrintJob printJob, out string additionalInfo)
        {
            OnUiUpdate(CardPrintingLogicResource.FetchPrintJobUi, false, true, EventArgs.Empty);

            printJob = null;

            PrinterInfo printerInfo = new PrinterInfo
            {
                FirmwareVersion = _printer.FirmwareVersion,
                Manufacturer = _printer.Manufacturer,
                Model = _printer.Model,
                SerialNo = _printer.DeviceId
            };


            var printJobResp = _cardPrinting.GetPrintJob(token, printerInfo);

            if (printJobResp.Success)
            {
                printJob = printJobResp.Value;
            }

            additionalInfo = printJobResp.AdditionalInfo;

            return printJobResp.Success;
        }

        public bool SetPrinterSetting(IPrintJob printJob, out string additionalInfo)
        {
            OnUiUpdate(CardPrintingLogicResource.PrinterOptionsUi, false, true, EventArgs.Empty);

            _printer.SetDeviceSettings(printJob.AppOptionToDictionary());

            additionalInfo = string.Empty;
            return true;
        }

        public bool StartPhysicalPrint(IPrintJob printJob, out CardData cardData, out string additionalInfo)
        {
            OnUiUpdate(CardPrintingLogicResource.BuildPrinterJobUi, false, true, EventArgs.Empty);

            cardData = null;
            additionalInfo = string.Empty;

            var cardPrintDetails = _printer.PrinterDetailFactory().Populate(printJob.ProductFields);
            int printJobSuccess;
            Device.IDeviceMagData magData = null;

            if (printJob.MustReturnCardData)
            {
                printJobSuccess = _printer.ReadAndPrint(printJob.ProductBin, cardPrintDetails, out magData);
            }
            else
            {
                printJobSuccess = _printer.Print(printJob.ProductBin, cardPrintDetails);
            }

            if (printJobSuccess == PrinterCodes.Success && magData != null)
            {
                var track2 = magData.TrackDataToString(2);
                var sepPos = track2.IndexOfAny(new char[] { '=', 'D' });

                string ecryptedPAN = EncryptionManager.EncryptString(track2.Substring(0, sepPos),
                                                                  Veneka.Indigo.Common.Utilities.StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                  Veneka.Indigo.Common.Utilities.StaticFields.EXTERNAL_SECURITY_KEY);
                cardData = new CardData
                {
                    Track2 = track2,
                    PAN = ecryptedPAN
                };
            };

            if(printJobSuccess == PrinterCodes.ProductBinAndCardMismatch)
            {
                additionalInfo = "Card Product BIN does not match BIN on card used for printing";
            }
            else if (printJobSuccess == PrinterCodes.PrintJobCancelled)
            {
                additionalInfo = "Card Jammed in Printer while printing.";

            }

            return printJobSuccess.Equals(PrinterCodes.Success);
        }

    }
}
