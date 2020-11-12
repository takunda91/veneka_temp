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
using System.Runtime.InteropServices;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.DesktopApp.Device.Printer;
using Veneka.Indigo.UX.NativeAppAPI;
using Veneka.Indigo.DesktopApp.Logic;
using System.Threading.Tasks;
using System.Threading;

namespace IndigoDesktopApp.ViewModels
{
    public class PrintViewModel : IPageViewModel, INotifyPropertyChanged
    {
        private ICommand deviceInfoCommand;
        private ICommand printCommand;
        private ICommand cancelCommand;
        private ICommand refreshCommand;
        
        private bool canExecute = true;
        private bool canPrint = false;
        private bool canDeviceInfo = false;
        private bool canCancel = false;

        private IPrinter[] _printers = new IPrinter[0];
        private IPrinter _selectedPrinter;
        private string _deviceInfo = String.Empty;

        private string _info = String.Empty;
        private string _error = String.Empty;


        public PrintViewModel()
        {
            DeviceInfoCommand = new RelayCommand(GetDeviceInfo, param => this.canDeviceInfo);
            PrintCommand = new RelayCommand(DoPrint, param => this.canPrint);            
            RefreshCommand = new RelayCommand(DoRefresh, param => this.canExecute);
            CancelCommand = new RelayCommand(DoCancel, param => this.canCancel);

            DoRefresh(null);
        }

        public IPrinter[] Printers
        {
            get { return _printers; }
            private set
            {
                _printers = value;
                OnPropertyChanged("Printers");
            }
        }


        public IPrinter SelectedPrinter
        {
            get { return _selectedPrinter; }
            private set
            {
                _selectedPrinter = value;
                OnPropertyChanged("SelectedPrinter");
            }
        }

        public void GetDeviceInfo(object obj)
        {
            if (obj is IPrinter)
            {
                var printer = (IPrinter)obj;

                try
                {
                    printer.Connect();
                    var printerInfo = printer.GetPrinterInfo();


                    var infoStr = new StringBuilder();
                    foreach(var item in printerInfo)
                    {
                        infoStr.AppendFormat("{0} - {1}{2}", item.Key, item.Value, Environment.NewLine);
                    }
                    DeviceInfo = infoStr.ToString();

                    printer.Disconnect();
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    printer.Dispose();
                }
            }
        }

        public void DoRefresh(object obj)
        {
            try
            {
                canDeviceInfo = false;
                canPrint = false;
                PrinterSearch search = new PrinterSearch();
                var devices = search.SearchForConnectedPrinters();

                if (devices.Count() > 0)
                {
                    Printers = devices;
                    SelectedPrinter = devices[0];
                    canDeviceInfo = true;
                    canPrint = StartupProperties.Action == StartupProperties.Actions.PrintCard && !string.IsNullOrEmpty(StartupProperties.Token);
                }
                else
                {
                    Printers = new IPrinter[0];
                }
            }
            catch (Exception ex)
            {
                Error = ex.StackTrace;
                Printers = new IPrinter[0];
            }
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

        public void DoPrint(object obj)
        {
            if (obj is IPrinter)
            {
                var printer = (IPrinter)obj;
                var ui = TaskScheduler.FromCurrentSynchronizationContext();

                //TestPinOperations testPinOperations = new TestPinOperations(this);
                NativeAppClient client = new NativeAppClient(StartupProperties.URL);

                var cardPrintLogic = new CardPrintingLogic(client, printer);
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

        public void DoCancel(object obj)
        {

        }

        public bool CanExecute
        {
            get { return this.canExecute; }
            set
            {
                if (this.canExecute == value)
                    return;

                this.canExecute = value;
            }
        }

        public ICommand DeviceInfoCommand
        {
            get { return deviceInfoCommand; }
            set { deviceInfoCommand = value; }
        }

        public ICommand CancelCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value; }
        }

        public ICommand PrintCommand
        {
            get { return printCommand; }
            set { printCommand = value; }
        }
        
        public ICommand RefreshCommand
        {
            get { return refreshCommand; }
            set { refreshCommand = value; }
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
    }
}
