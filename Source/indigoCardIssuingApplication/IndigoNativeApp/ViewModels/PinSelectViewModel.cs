using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Veneka.Indigo.DesktopApp.Device;
using Veneka.Indigo.DesktopApp.Device.PINPad;
using Veneka.Indigo.DesktopApp.Device.PINPad.PAXS300;
using Veneka.Indigo.DesktopApp.Logic;
using Veneka.Indigo.UX.NativeAppAPI;

namespace IndigoDesktopApp.ViewModels
{
    public class PinSelectViewModel : IPageViewModel, INotifyPropertyChanged
    {
        private ICommand startPINCommand;
        private ICommand refreshCommand;
        private bool canExecute = true;

        private string _token = "";
        private IPINPad[] _pinPads = new IPINPad[0];
        private IPINPad _selectedPINPad = null;

        private string _error = String.Empty;
        private string _info = String.Empty;

        public PinSelectViewModel()
        {
            StartPINCommand = new RelayCommand(DoPINSelect, param => this.canExecute);
            RefreshCommand = new RelayCommand(DoRefresh, param => this.canExecute);

            try
            {
                _token = StartupProperties.URL.ToString();
            }
            catch(Exception ex)
            {
                _token = ex.Message;
            }

            PINPadSearch search = new PINPadSearch();
            var devices = search.SearchForConnectedPINPads();

            if (devices.Count() > 0)
                PINPads = devices;
            else
                PINPads = new IPINPad[0];
        }

        
        public IPINPad SelectedPINPad
        {
            get { return _selectedPINPad; }
            private set
            {
                _selectedPINPad = value;
                OnPropertyChanged("SelectedPINPad");
            }
        }

        public IPINPad[] PINPads
        {
            get { return _pinPads; }
            private set
            {
                _pinPads = value;
                OnPropertyChanged("PINPads");
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

        public void DoRefresh(object obj)
        {
            PINPadSearch search = new PINPadSearch();
            var devices = search.SearchForConnectedPINPads();

            if (devices.Count() > 0)
            {
                PINPads = devices;
                SelectedPINPad = devices[0];
            }
            else
                PINPads = new IPINPad[0];
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

        public void DoPINSelect(object obj)
        {
            // COMP1 DA52 75B0 3808 D031 E5D5 8394 86BF 5489
            // COMP2 8664 2349 0B62 8F4C 64E0 0725 577C 9EEA
            // COMP3 4338 E5B5 AB40 92CE 5D91 0E4C 4662 2A13

            //TEST TPK: X6A3FDB0309D54300E4D15526A1547454    
            //          UD019BA0D484B9968CB186F183B843620
            //          TMK (clear): U5D3757F8326B5E7C803485B0D0C2CB62
            //          New key (clear): 31BAAEA801CD0EADB9DAFED0CDF240E6
            //          New key(TMK): X6A3FDB0309D54300E4D15526A1547454
            //           New key(LMK): UD019BA0D484B9968CB186F183B843620


            if (obj is IPINPad)
            {
                var pinPad = (IPINPad)obj;
                var ui = TaskScheduler.FromCurrentSynchronizationContext();

                //TestPinOperations testPinOperations = new TestPinOperations(this);
                NativeAppAPI.NativeAppClient _client = new NativeAppAPI.NativeAppClient(StartupProperties.URL);

                PINSelectLogic pinSelect = new PINSelectLogic(_client, pinPad);
                pinSelect.UiUpdate += new UiUpdateEventHandler(OnUiUpdate);

                CancellationTokenSource cts = new CancellationTokenSource();

                Task.Factory.StartNew(() =>
                {

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
                        Error = String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.ToString());
                    }
                    finally
                    {
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

        public ICommand StartPINCommand
        {
            get { return startPINCommand; }
            set { startPINCommand = value; }
        }

        public ICommand RefreshCommand
        {
            get { return refreshCommand; }
            set { refreshCommand = value; }
        }

        public string Token
        {
            get { return _token; }
            set
            {
                _token = value;
                OnPropertyChanged("Token");
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
