using Veneka.Indigo.DesktopApp.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.DesktopApp.Device.PINPad;
using IndigoDesktopApp.NativeAppAPI;
using Veneka.Indigo.DesktopApp.Device.Printer.Zebra;
using System.Windows.Input;
using System.ComponentModel;
using Veneka.Indigo.DesktopApp.Device.Printer;
using System.Threading.Tasks;
using System.Threading;
using Veneka.Indigo.DesktopApp.Logic;
using Veneka.Indigo.UX.NativeAppAPI;

namespace IndigoDesktopApp.ViewModels
{
    public class DevicesViewModel : IPageViewModel, INotifyPropertyChanged
    {
        private ICommand deviceInfoCommand;
        private ICommand printTestCommand;
        private ICommand pinTestCommand;
        private ICommand magReadTestCommand;
        private ICommand deviceSearchCommand;
        private ICommand cancelCommand;

        private bool _canDeviceSearch = true;
        private bool _canPinTest = false;
        private bool _canDeviceInfo = false;
        private bool _canPrintTest = false;
        private bool _canMagReadTest = false;
        private bool _canCancel = false;

        private IDevice[] _devices = new IDevice[0];
        private string _deviceInfo = String.Empty;
        private string _error = String.Empty;
        private string _info = String.Empty;

        public DevicesViewModel()
        {
            DeviceInfoCommand = new RelayCommand(DoGetDeviceInfo, param => this._canDeviceInfo);
            PrintTestCommand = new RelayCommand(DoPrintTest, param => this._canPrintTest);
            MagReadTestCommand = new RelayCommand(DoMagReadTest, param => this._canMagReadTest);
            PinTestCommand = new RelayCommand(DoPinSelectTest, param => this._canPinTest);
            DeviceSearchCommand = new RelayCommand(DoDeviceSearch, param => this._canDeviceSearch);
            CancelCommand = new RelayCommand(DoCancel, param => this._canCancel);

            DeviceInfo = "";



            //NativeAppClient client = new NativeAppClient("https://localhost/NativeAPI.svc/soap");

            //client.PINSignOn("Hello", "This is my password");

            //DeviceSearch search = new DeviceSearch();
            //var devices = search.GetUSBDevices();

            //if (devices.Count() > 0)
            //{
            //    Device = devices[0].Manufacturer + " " + devices[0].Name + " on COM-" + devices[0].ComPort;

            //    if (devices[0] is IPINPad)
            //        ((IPINPad)devices[0]).InitialisePinPad();

            //}
            //else
            //    Device = "No Device";
        }

        private void OnUiUpdate(object sender, string message, bool isError, bool append, EventArgs e)
        {
            if (isError)
            {
                Error = message;
            }
            else
            {
                Info = message;
            }
        }

        public void DoDeviceSearch(object obj)
        {
            CanDeviceInfo =
            CanPinTest = 
            CanPrintTest = 
            CanDeviceSearch = false;
            CommandManager.InvalidateRequerySuggested();

            KeyValuePair<int, string> selectedItem;

            if (obj is KeyValuePair<int, string>)
                selectedItem = (KeyValuePair<int, string>)obj;
            else throw new Exception();

            Task.Factory.StartNew(() =>
            {
                Error = String.Empty;
                Info = "Searching for Devices";
                if (selectedItem.Key == 0)
                {
                    var printerSearch = new PrinterSearch();
                    Devices = printerSearch.SearchForConnectedPrinters();
                }
                else if (selectedItem.Key == 1)
                {
                    var pinPadSearch = new PINPadSearch();
                    Devices = pinPadSearch.SearchForConnectedPINPads();
                }
                else
                {

                }

                if(Devices.Length > 0)
                {
                    CanDeviceInfo = true;
                    CanPinTest = selectedItem.Key == 1;
                    CanPrintTest = selectedItem.Key == 0;
                }

                Info = "Done Searching";
            })
            .ContinueWith(manifest =>
            {
                CanDeviceSearch = true;
                CommandManager.InvalidateRequerySuggested();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void DoGetDeviceInfo(object obj)
        {
            CanDeviceSearch =
            CanPinTest =
            CanPrintTest =
            CanDeviceInfo = false;
            CommandManager.InvalidateRequerySuggested();

            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            CancellationTokenSource cts = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                if(obj == null)
                {
                    Info = "Please select Device";
                    return;
                }

                try
                {
                    Error = String.Empty;
                    Info = "Getting Device Info.";
                    if (obj is IPrinter)
                    {
                        using (var printer = (IPrinter)obj)
                        {
                            printer.Connect();
                            DeviceInfo = printer.ToString();
                        }
                    }
                    else if (obj is IPINPad)
                    {
                        using (var pinPad = (IPINPad)obj)
                        {
                            pinPad.InitialisePinPad();
                            DeviceInfo = pinPad.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Error = String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.ToString());
                }
                finally
                {
                    CanPinTest = obj is IPINPad;
                    CanPrintTest = obj is IPrinter;
                    Info = "Done Getting Device Info";
                }

            },
                cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.None,
                scheduler: TaskScheduler.Default)
                .ContinueWith(manifest =>
                {
                    CanDeviceSearch = 
                    CanDeviceInfo = true;
                    CommandManager.InvalidateRequerySuggested();

                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void DoPrintTest(object obj)
        {
            if (obj is IPrinter)
            {
                var printer = (IPrinter)obj;
                var ui = TaskScheduler.FromCurrentSynchronizationContext();

                //TestPinOperations testPinOperations = new TestPinOperations(this);
                // NativeAppClient client = new NativeAppClient(StartupProperties.URL);
                TestPrintingOperations testPrintOps = new TestPrintingOperations();

                var cardPrintLogic = new CardPrintingLogic(testPrintOps, printer);
                cardPrintLogic.UiUpdate += new UiUpdateEventHandler(OnUiUpdate);

                CancellationTokenSource cts = new CancellationTokenSource();

                Task.Factory.StartNew(() =>
                {

                    try
                    {
                        cardPrintLogic.DoPrintJob();
                    }
                    catch (System.ServiceModel.EndpointNotFoundException enfex)
                    {
                        Error = enfex.Message;
                    }
                    catch (Exception ex)
                    {
                        Error = String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.ToString());
                    }
                    finally
                    {
                        printer.Dispose();
                    }
                },
                cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.None,
                scheduler: TaskScheduler.Default)
                .ContinueWith(manifest =>
                {
                    CommandManager.InvalidateRequerySuggested();

                }, TaskScheduler.FromCurrentSynchronizationContext());
                //.ContinueWith(task =>
                //{

                //}, TaskContinuationOptions.NotOnFaulted);
            }
        }

        public void DoMagReadTest(object obj)
        {

        }

        public void DoPinSelectTest(object obj)
        {
            if (obj is IPINPad)
            {
                var pinPad = (IPINPad)obj;
                var ui = TaskScheduler.FromCurrentSynchronizationContext();

                TestPinOperations testPinOperations = new TestPinOperations(this);

                PINSelectLogic pinSelect = new PINSelectLogic(testPinOperations, pinPad);
                pinSelect.UiUpdate += new UiUpdateEventHandler(OnUiUpdate);

                CancellationTokenSource cts = new CancellationTokenSource();

                Task.Factory.StartNew(() =>
                {
                    Error = String.Empty;
                    Info = "Starting PIN Select Test";
                    try
                    {
                        pinSelect.DoPINSelect();
                    }
                    catch (System.ServiceModel.EndpointNotFoundException enfex)
                    {
                        Error = enfex.Message;                        
                    }
                    catch (Exception ex)
                    {
                        Error = ex.Message;
                    }
                    finally
                    {
                        Info = "Finished PIN Select Test";
                        pinPad.Dispose();
                    }
                },
                cancellationToken: CancellationToken.None,
                creationOptions: TaskCreationOptions.None,
                scheduler: TaskScheduler.Default)
                .ContinueWith(manifest =>
                {
                    CommandManager.InvalidateRequerySuggested();

                }, TaskScheduler.FromCurrentSynchronizationContext());
                //.ContinueWith(task =>
                //{

                //}, TaskContinuationOptions.NotOnFaulted);
            }
        }

        public void DoCancel(object obj)
        {

        }

        

        public bool CanDeviceInfo
        {
            get { return _canDeviceInfo; }
            set
            {
                if (this._canDeviceInfo == value)
                    return;

                this._canDeviceInfo = value;
                OnPropertyChanged("CanDeviceInfo");
            }
        }

        public bool CanDeviceSearch
        {
            get { return _canDeviceSearch; }
            set
            {
                if (this._canDeviceSearch == value)
                    return;

                this._canDeviceSearch = value;
                OnPropertyChanged("CanDeviceSearch");
            }
        }

        public bool CanPinTest
        {
            get { return _canPinTest; }
            set
            {
                if (this._canPinTest == value)
                    return;

                this._canPinTest = value;
                OnPropertyChanged("CanPinTest");
            }
        }

        public bool CanPrintTest
        {
            get { return this._canPrintTest; }
            set
            {
                if (this._canPrintTest == value)
                    return;

                this._canPrintTest = value;
                OnPropertyChanged("CanPrintTest");
            }
        }

        public ICommand DeviceInfoCommand
        {
            get { return deviceInfoCommand; }
            set { deviceInfoCommand = value; }
        }

        public ICommand PrintTestCommand
        {
            get { return printTestCommand; }
            set { printTestCommand = value; }
        }

        public ICommand MagReadTestCommand
        {
            get { return magReadTestCommand; }
            set { magReadTestCommand = value; }
        }

        public ICommand PinTestCommand
        {
            get { return pinTestCommand; }
            set { pinTestCommand = value; }
        }

        public ICommand DeviceSearchCommand
        {
            get { return deviceSearchCommand; }
            set { deviceSearchCommand = value; }
        }

        public ICommand CancelCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value; }
        }

        public KeyValuePair<int, string>[] DeviceTypeList
        {
            get
            {
                return new KeyValuePair<int, string>[]
                {
                    new KeyValuePair<int, string>(0, "Card Printer"),
                    new KeyValuePair<int, string>(1, "PIN Pad")
                };
            }
        }

        public IDevice[] Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        public string DeviceInfo
        {
            get { return _deviceInfo; }
            set
            {
                _deviceInfo = value;
                OnPropertyChanged("DeviceInfo");
            }
        }

        public string Info
        {
            get { return _info; }
            set
            {
                _info = value;
                OnPropertyChanged("Info");
            }
        }

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                OnPropertyChanged("Error");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class TestPinOperations : Veneka.Indigo.UX.NativeAppAPI.IPINOperations
        {
            private readonly DevicesViewModel _deviceView;
            public TestPinOperations(DevicesViewModel deviceView)
            {
                _deviceView = deviceView;
            }

            public Response<string> Complete(CardData cardData, Token token)
            {
                _deviceView.Error = String.Format("PAN={0}, Track2={1}", cardData.PAN, cardData.Track2);

                return new Response<string>(true, "", "", "");
            }

            public Response<ProductSettings> GetProductConfig(CardData cardData, Token token)
            {
                _deviceView.Error = String.Format("PAN={0}, Track2={1}", cardData.PAN, cardData.Track2);

                ProductSettings productSettings = new ProductSettings();
                productSettings.MaxPINLength = 4;
                productSettings.MinPINLength = 4;                

                return new Response<ProductSettings>(true, "", productSettings, "");
            }

            public Response<string[]> GetWorkingKey(Token token)
            {
                //B1A3B4F6E1CC6708F41AC059040E4A6B
                //X6A3FDB0309D54300E4D15526A1547454
                string[] keys = new string[] { "XB1A3B4F6E1CC6708F41AC059040E4A6B" };

                return new Response<string[]>(true, "", keys, "");
            }

            public Response<string> Logon(string username, string password)
            {
                throw new NotImplementedException();
            }
        }

        public class TestPrintingOperations : Veneka.Indigo.UX.NativeAppAPI.ICardPrinting
        {
            private readonly DevicesViewModel _deviceView;

            public Response<PrintJob> GetPrintJob(Token printToken, PrinterInfo printer)
            {
                return new Response<PrintJob>
                {
                    Success = true,
                    AdditionalInfo = "Additional Info",
                    Session = "Session",
                    Value =
                    new PrintJob
                    {
                        ApplicationOptions = new AppOptions[0],
                        MustReturnCardData = false,
                        PrintJobId = "printjobid",
                        ProductBin = "123456",
                        ProductFields = new Veneka.Indigo.Integration.ProductPrinting.ProductField[]
                        {
                            new Veneka.Indigo.Integration.ProductPrinting.ProductField
                            {
                                Deleted = false,
                                Font = "Arial",
                                FontColourRGB = 1,
                                Printable = true,
                                PrintSide = 0,
                                ProductPrintFieldTypeId = 0,
                                Value = Encoding.UTF8.GetBytes("Some Text"),
                                X = 50,
                                Y = 100
                            }
                        }
                    }
                };
            }

            public Response<string> PrinterAnalytics(Token printToken)
            {
                return new Response<string>
                {
                    Success = true,
                    AdditionalInfo = "Additional Info",
                    Session = "PrinterAnalytics",
                    Value = ""                    
                };
            }

            public Response<string> PrinterAuditDetails(Token printToken, PrinterInfo printer)
            {
                return new Response<string>
                {
                    Success = true,
                    AdditionalInfo = "Additional Info",
                    Session = "PrinterAuditDetails",
                    Value = ""
                };
            }

            public Response<string> PrintFailed(Token printToken, string comments)
            {
                return new Response<string>
                {
                    Success = true,
                    AdditionalInfo = "Additional Info",
                    Session = "PrintFailed",
                    Value = ""
                };
            }

            public Response<string> PrintingComplete(Token printToken, CardData cardData)
            {
                return new Response<string>
                {
                    Success = true,
                    AdditionalInfo = "Additional Info",
                    Session = "PrintingComplete",
                    Value = ""
                };
            }

            public Response<string> SendToPrinter(Token printToken)
            {
                return new Response<string>
                {
                    Success = true,
                    AdditionalInfo = "Additional Info",
                    Session = "SendToPrinter",
                    Value = ""
                };
            }
        }
    }
}
