


using Common.Logging;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Veneka.Indigo.BackOffice.API;
using Veneka.Indigo.BackOffice.Application.Authentication;
using Veneka.Indigo.BackOffice.Application.BackOfficeApiClient;
using Veneka.Indigo.BackOffice.Application.Crypto;
using Veneka.Indigo.BackOffice.Application.Database;
using Veneka.Indigo.BackOffice.Application.Devices;
using Veneka.Indigo.BackOffice.Application.Devices.Zebra;
using Veneka.Indigo.BackOffice.Application.Objects;
using Veneka.Indigo.BackOffice.Application.Properties;

namespace Veneka.Indigo.BackOffice.Application.ViewModels
{
    public class BulkPrintingViewModel : ViewModel
    {
        private Devices.DevicesManager _devicesMan = new Devices.DevicesManager();
       
        private readonly DelegateCommand _printCommand;
        private readonly DelegateCommand _printingCommand;

        private readonly DelegateCommand _searchDeviceCommand;
        private readonly DelegateCommand _startCommand;
        private readonly DelegateCommand _continueCommand;
        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _reprintCommand;
        private readonly DelegateCommand _spoilCommand;
        private readonly DelegateCommand _uploadCommand;
        private readonly DelegateCommand _viewReportCommand;

        
        private PrintBatchData _selectedPrint = null;

        private ObservableCollection<PrintBatchData> _batches;
        private ObservableCollection<RequestData> _requests;
        private WindowsCrypto winCrypt;
        private ObservableCollection<RequestData> _requeststoupload;

        private List<string> _spoiledCards;
        private string apiToken;
        public BackOfficeClient _backOfficeClient;
        private bool _showContinueButton;
        private bool _showPrintButton;
        private bool _showReprintButton;
        private bool _showSpoilButton;
        private int _startindex;
        private int _cardsinbatch;
        private int _noOfRequests;
        private bool _showCancelButton;
        private bool _showUploadButton;
        private bool _firstColumnChecked;
        private BackOfficeDAL _backofficedal;
        private ZebraZXP7 zebra;
        private int _keysize = 16;
        private ICommand doSomethingCommand;


        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public List<String> Results { get; set; }
        public bool IsBusy { get; set; }
        private static readonly ILog log = LogManager.GetLogger(typeof(BulkPrintingViewModel));


        public BulkPrintingViewModel()
        {
            _backOfficeClient = new BackOfficeClient(new Uri(Uri));
            _backofficedal = BackOfficeDAL.Instance;
            _showContinueButton = false;
            _showPrintButton = true;
            _showReprintButton = false;
            _showSpoilButton = false;
            _showCancelButton = true;
            _showUploadButton = true;


            _spoiledCards = new List<string>();

            if (IsAuthenticated)
            {
                BackOfficeAppPrincipal _back = (BackOfficeAppPrincipal)Thread.CurrentPrincipal;
                apiToken = _back.Identity.Token;
            }
            var resp = _backOfficeClient.GetApprovedPrintBatches(apiToken);

            if (resp.Success)
            {
                Batches = new ObservableCollection<PrintBatchData>(resp.Value);

                if (resp.Value != null && resp.Value.Count > 0)
                {
                    _selectedPrint = resp.Value[0];
                  
                }

            }

            _printCommand = new DelegateCommand(Print, CanPrint);
            _printingCommand = new DelegateCommand(Printing, CanPrint);
            _reprintCommand = new DelegateCommand(Reprint);
            _spoilCommand = new DelegateCommand(Spoil);
            _uploadCommand = new DelegateCommand(Upload);

            _viewReportCommand = new DelegateCommand(ViewReport);
            _searchDeviceCommand = new DelegateCommand(SearchDevice, null);
            _startCommand = new DelegateCommand(Start, null);
            _continueCommand = new DelegateCommand(Continue, CanContinue);
            _cancelCommand = new DelegateCommand(Cancel, null);

            NotifyPropertyChanged("ShowContinueButton");
            NotifyPropertyChanged("ShowPrintButton");
            NotifyPropertyChanged("ShowReprintButton");
            NotifyPropertyChanged("ShowSpoilButton");
            NotifyPropertyChanged("ShowCancelButon");
            NotifyPropertyChanged("ShowUploadButton");
            _requeststoupload = new ObservableCollection<RequestData>();
             DeviceTypes = new ObservableCollection<KeyValuePair<int, string>>()
            {
               new KeyValuePair<int, string>(0 ,"USB"),
               new KeyValuePair<int, string>(1 ,"Network")
            };
        }

        public ICommand DoSomething
        {
            get { return doSomethingCommand ?? (doSomethingCommand = new DelegateCommand(LongRunningTask)); }
        }

        private void LongRunningTask(object obj)
        {
            //var task = new Task(SearchDevice(obj));
            //task.Start();
        }


        #region Commands


        public DelegateCommand ViewReportCommand { get { return _viewReportCommand; } }

        public DelegateCommand PrintingCommand { get { return _printingCommand; } }

        public DelegateCommand PrintCommand { get { return _printCommand; } }
        public DelegateCommand SearchDeviceCommand { get { return _searchDeviceCommand; } }
        public DelegateCommand StartCommand { get { return _startCommand; } }
        public DelegateCommand ContinueCommand { get { return _continueCommand; } }
        public DelegateCommand CancelCommand { get { return _cancelCommand; } }
        public DelegateCommand ReprintCommand { get { return _reprintCommand; } }
        public DelegateCommand SpoilCommand { get { return _spoilCommand; } }
        public DelegateCommand UploadCommand { get { return _uploadCommand; } }
       

       


        #endregion

        #region Properties
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        public bool ShowContinueButton
        {
            get { return _showContinueButton; }
            set { _showContinueButton = value;
                OnPropertyChanged("ShowContinueButton");
            }
        }
        public bool FirstColumnChecked
        {
            get { return _firstColumnChecked; }
            set
            {
                _firstColumnChecked = value;
                OnPropertyChanged(nameof(FirstColumnChecked));
            }
        }
      
        public bool ShowReprintButton
        {
            get { return _showReprintButton; }
            set
            {
                _showReprintButton = value;
                OnPropertyChanged("ShowReprintButton");
            }
        }
        public bool ShowSpoilButton
        {
            get { return _showSpoilButton; }
            set
            {
                _showSpoilButton = value;
                OnPropertyChanged("ShowSpoilButton");
            }
        }
        public bool ShowPrintButton
        {
            get { return _showPrintButton; }
            set { _showPrintButton = value;
                OnPropertyChanged("ShowPrintButton");
            }

        }

        public bool ShowCancelButon
        {
            get { return _showCancelButton; }
            set
            {
                _showCancelButton = value;
                OnPropertyChanged("ShowCancelButon");
            }

        }

        public bool ShowUploadButton
        {
            get { return _showUploadButton; }
            set
            {
                _showUploadButton = value;
                OnPropertyChanged("ShowUploadButton");
            }

        }
        public ObservableCollection<PrintBatchData> Batches
        {
            get
            {
                return _batches;
            }
            set
            {
                if (_batches != value)
                {
                    _batches = value;
                    OnPropertyChanged(nameof(Batches));
                }
            }
        }

        public ObservableCollection<RequestData> Requests
        {
            get
            {
                return _requests;
            }
            set
            {
                if (_requests != value)
                {
                    _requests = value;
                    OnPropertyChanged(nameof(Requests));
                    
                }
            }
        }
        public ObservableCollection<RequestData> RequestsToUpload
        {
            get
            {
                return _requeststoupload;
            }
            set
            {
                if (_requeststoupload != value)
                {
                    _requeststoupload = value;
                    OnPropertyChanged(nameof(RequestsToUpload));
                }
            }
        }


        public PrintBatchData SelectedPrint
        {
            get
            {
                return _selectedPrint;
            }
            set
            {
                _selectedPrint = value;
            }
        }
        #endregion

        #region Private Methods

        private void ViewReport(object obj)
        {
          DataTable dt=_backofficedal.GetRequests(SelectedPrint.PrintBatchId);

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".pdf",
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*"
            };
            bool? fileOpenResult = openFileDialog.ShowDialog();
           
              
            string fileName = string.Empty;

            if (fileOpenResult != true)
            {
                fileName = openFileDialog.FileName;
                byte[] content = PrintBatchReport.GenerateReport(dt);

               // Write out PDF from memory stream.
                using (FileStream pdfFile = new FileStream(@"c:\temp\report_"+DateTime.Now.ToString("ddmmyyyhhmmss"), FileMode.CreateNew, FileAccess.Write))
                {
                    pdfFile.Write(content, 0, (int)content.Length);
                    pdfFile.Close();
                }

            }

            }
        private void Printing(object parameter)
        {
            try
            {
               
                DataGrid dgrequest = (DataGrid)parameter;
                foreach (RequestData row in Requests.Where(i => i.IsSelected = true))
                {
                    row.RequestStatus = "Printing";
                    row.PrintingProgress = PrintingStatus.PRINTING.ToString();
                    dgrequest.Items.Refresh();
                   

                    SentToPrinter(row, PrintingStatus.PRINTING, PrintingStatus.PRINTED);



                    dgrequest.Items.Refresh();

                }
                var result = new ObservableCollection<RequestData>(_requeststoupload.Concat(_requests));
                /// add the requests after  printing
                _requeststoupload = result;
                ShowContinueButton = true;
                ShowPrintButton = false;
                ShowReprintButton = true;
                ShowSpoilButton = true;
                ShowCancelButon = false;
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                MessageBox.Show(ex.Message+":"+ex.StackTrace);
            }
            
        }
       private void SentToPrinter(RequestData row, PrintingStatus previousStatus,PrintingStatus newStatus)
        {
            string _msg = string.Empty;
            string _track1Data = string.Empty;
            string _track2Data = string.Empty;
            string _track3Data = string.Empty;

            if (!_backofficedal.IsRequestInProgress(row.RequestId, (long)_selectedPrint.PrintBatchId, row.PrintingProgress))
            {
                _backofficedal.InsertRequests(row.RequestId, (long)_selectedPrint.PrintBatchId, row.PrintingProgress,row.RequestReference,row.RequestStatuesId);
            }

            List<PrintField> _list = new List<PrintField>();
            if (zebra != null)
            {
                try
                {
                    zebra.Disconnect();
                    zebra.Connect();
                    if (zebra.DeviceName.ToUpper() != "DEVICE 1")
                    {
                        /// send to printer get card number 
                        zebra.MagRead(out _track1Data, out _track2Data, out _track3Data, out _msg);

                        foreach (ProductTemplate item in row.ProdTemplateList)
                        {
                            log.Debug(item.Value);
                            _list.Add(new PrintField() { Value = item.Value, Font = item.font, FontColourRGB = int.Parse(item.fontColourRGB), FontSize = item.font_size, X = item.x, Y = item.y, PrintFieldTypeId = item.productPrintFieldTypeId, PrintSide = item.PrintSide });
                        }

                       
                        //printing
                        if (zebra.Print(_list.ToArray(), out _msg))
                        {
                            row.PAN = _track2Data.Split('=')[0];
                            MessageBox.Show(row.PAN);
                        }
                    }
                    else
                    {
                        row.PAN = "8098090909090909";
                    }
                        MessageBox.Show(_msg);
                    log.Debug(_msg);

                    byte[] pan = winCrypt.Encrypt(row.PAN);
                    row.RequestStatus = newStatus.ToString();
                    row.PrintingProgress = newStatus.ToString();
                    _backofficedal.UpdateRequest(row.RequestId, (long)_selectedPrint.PrintBatchId, PrintingStatus.PRINTING.ToString(), row.PrintingProgress, Base64.ToBase64String(pan));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    zebra.Disconnect();
                }
            }
            
           
            //row.PAN = "09980989900909";

            //byte[] pan = winCrypt.Encrypt(row.PAN);

            //_backofficedal.UpdateRequest(row.RequestId, (long)_selectedPrint.PrintBatchId, previousStatus.ToString(), newStatus.ToString(), Org.BouncyCastle.Utilities.Encoders.Base64.ToBase64String(pan));


        }
        private void Upload(object Parms)
        {
            try
            {
                UpdatePrintBatchDetails _printbatchdata = new UpdatePrintBatchDetails();
                _printbatchdata.PrintBatchStatusId = _selectedPrint.PrintBatchStatusId;
                _printbatchdata.Successful = true;
                _printbatchdata.PrintBatchId = int.Parse(_selectedPrint.PrintBatchId.ToString());

                List<RequestDetails> _requestDetails = new List<RequestDetails>();
                DataTable dt = _backofficedal.GetRequests(SelectedPrint.PrintBatchId);

                List<string> _list = new List<string>();
                foreach (DataRow dr in dt.Select("printing_status='" + PrintingStatus.PRINTED.ToString() + "'"))
                {
                    string PAN = winCrypt.Decrypt(Base64.Decode(dr["pan"].ToString()));
                    _requestDetails.Add(new RequestDetails() { PAN = PAN, RequestReference = dr["request_reference"].ToString(), RequestId = long.Parse(dr["request_id"].ToString()), RequestStatuesId = 1 });//flag 1 is successful. and 0 is failed. 
                }
                foreach (DataRow dr in dt.Select("printing_status='" + PrintingStatus.RE_PRINT.ToString() + "'"))
                {
                    string PAN = winCrypt.Decrypt(Base64.Decode(dr["pan"].ToString()));
                    _requestDetails.Add(new RequestDetails() { PAN = PAN, RequestReference = dr["request_reference"].ToString(), RequestId = long.Parse(dr["request_id"].ToString()), RequestStatuesId = 1 });
                }
                foreach (DataRow dr in dt.Select("printing_status='" + PrintingStatus.CARD_SPOILED.ToString() + "'"))
                {
                    string PAN = winCrypt.Decrypt(Base64.Decode(dr["pan"].ToString()));

                    _list.Add(PAN);
                }
                _printbatchdata.RequestDetails = _requestDetails;
                _printbatchdata.Cardstobespoiled = _list;
                _backOfficeClient.updatePrintBatchStatus(_printbatchdata, apiToken);
                SwitchView = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Print(object parameter)
        {

            SwitchView = 1;

        }

        private void SearchDevice(object parameter)
        {
            try
            {
                IsBusy = true;
                switch (((KeyValuePair<int, string>)parameter).Key)
                {
                    case 0:
                        Devices = _devicesMan.FindDevices(DeviceConnectionTypes.USB);
                        break;
                    case 1:
                        Devices = _devicesMan.FindDevices(DeviceConnectionTypes.Ethernet);
                        break;
                    default:
                        Devices = _devicesMan.FindDevices();
                        break;
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
       

        

        
        private void Start(object parameter)
        {
            var values = (CommandParameters)parameter;
            List<RequestData> _requestdata = new List<RequestData>();
            if (SelectedDevice != null)
            {
                //IDevice _printer = (IDevice)values.Device;
                zebra = (ZebraZXP7)SelectedDevice;

                string CardstobePrintedInBatch = values.Text;
                SwitchView = 2;
                _startindex = 1;
                _cardsinbatch = int.Parse(CardstobePrintedInBatch);
                _requestdata = GetRequestsforBatch(_cardsinbatch);
                /// Get Keys for the workStation
                string[] keys = null;
                var response = _backOfficeClient.GetWorkStationKey(Environment.MachineName, _keysize, apiToken);
                if (response.Success)
                {
                    keys = response.Value;
                }
                winCrypt = new WindowsCrypto(keys[0], keys[1]);
                Requests = new ObservableCollection<RequestData>(_requestdata);
                // get only  printing in progress records.

                bool flag = UpdateRequestsFromLocalStorage();


                _noOfRequests = ((GetPrintBatchDetails)_selectedPrint).NoOfRequests;
                //if all cards printed show the continue button.
                if (!flag)
                {
                    ShowContinueButton = true;
                    ShowPrintButton = false;
                }
                else
                {
                    ShowContinueButton = false;
                    ShowPrintButton = true;
                }

                ShowSpoilButton = false;
                ShowReprintButton = false;
                ShowCancelButon = true;


                _continueCommand.RaiseCanExecuteChanged();
            }
            else
            {
                MessageBox.Show("Device is not selected.");
            }

            
            
        }
        private bool UpdateRequestsFromLocalStorage()
        {
            bool flag = false;
            DataTable dt = _backofficedal.GetRequests(SelectedPrint.PrintBatchId);
            if (dt.Rows.Count > 0)
            {
                foreach (var req in Requests)
                {
                    DataRow[] dr = dt.Select("request_id='" + req.RequestId.ToString() + "'");

                    if (dr.Length > 0)
                    {
                        if (dr[0]["printing_status"].ToString() == PrintingStatus.PRINTED.ToString())
                        {
                            req.PAN = winCrypt.Decrypt(Org.BouncyCastle.Utilities.Encoders.Base64.Decode(dr[0]["PAN"].ToString()));
                            req.IsSelected = false;
                            req.RequestStatus = dr[0]["printing_status"].ToString();
                            req.PrintingProgress = dr[0]["printing_status"].ToString();
                           
                        }
                        
                    }
                }
            }
            flag = Requests.Any(i => i.IsSelected == true);
            return flag;
        }

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable<DataGridRow>;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }
        private void Continue(object parameter)
        {
            try
            {
                _startindex = _startindex + 1;
                int _endindex = _startindex * _cardsinbatch;
                if (_noOfRequests >= _endindex)
                {
                    var _requestdata = GetRequestsforBatch(_endindex);

                    Requests = new ObservableCollection<RequestData>(_requestdata);

                    bool flag = UpdateRequestsFromLocalStorage();

                    ShowContinueButton = false;
                    ShowPrintButton = true;
                    ShowSpoilButton = false;
                    ShowReprintButton = false;
                    ShowCancelButon = false;



                }
                else
                {
                    SwitchView = 3;
                }
                _continueCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
        private List<RequestData> GetRequestsforBatch(int _cardsinbatch)
        {
            List<RequestData> _requestdata = new List<RequestData>();
            var resp = _backOfficeClient.GetRequestsforBatch((long)_selectedPrint.PrintBatchId, _startindex, _cardsinbatch, apiToken);
            if (resp.Success)
            {
                foreach (RequestDetails r in resp.Value)
                {
                   
                    _requestdata.Add(new RequestData(r, string.Empty, true));

                }
               
            }
            return _requestdata;
        }
        private void Reprint(object param)
        {
            try
            {


                IEnumerable<RequestData> selectedData = Requests.Where(d => d.IsSelected);

                foreach (RequestData item in selectedData)
                {
                    _spoiledCards.Add(item.PAN);
                    byte[] pan = winCrypt.Encrypt(item.PAN);

                    //item.PrintingProgress = PrintingStatus.RE_PRINT.ToString();
                    // spoil the old pan  and new request update statu to re-print.
                    _backofficedal.UpdateRequest(item.RequestId, (long)_selectedPrint.PrintBatchId, PrintingStatus.PRINTED.ToString(), PrintingStatus.CARD_SPOILED.ToString(), Org.BouncyCastle.Utilities.Encoders.Base64.ToBase64String(pan));

                    SentToPrinter(item, PrintingStatus.PRINTED, PrintingStatus.RE_PRINT);
                }


                ShowContinueButton = true;
                ShowPrintButton = false;
                ShowSpoilButton = true;
                ShowReprintButton = true;
                ShowCancelButon = false;
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
        private void Spoil(object param)
        {
            try
            {
                IEnumerable<RequestData> selectedData = Requests.Where(d => d.IsSelected);

                foreach (RequestData item in selectedData)
                {
                    _spoiledCards.Add(item.PAN);
                    byte[] pan = winCrypt.Encrypt(item.PAN);

                    item.PrintingProgress = PrintingStatus.CARD_SPOILED.ToString();
                    _backofficedal.UpdateRequest(item.RequestId, (long)_selectedPrint.PrintBatchId, PrintingStatus.PRINTED.ToString(), item.PrintingProgress, Org.BouncyCastle.Utilities.Encoders.Base64.ToBase64String(pan));

                }

                ShowContinueButton = true;
                ShowPrintButton = false;
                ShowSpoilButton = true;
                ShowReprintButton = true;
                ShowCancelButon = false;
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
        private void Cancel(object parameter)
        {
            SwitchView = 0;
        }

        private bool CanPrint(object parameter)
        {
            return true;
        }

        private bool CanContinue(object parameter)
        {
            return true;
        }

        #endregion
        public int SwitchView
        {
            get { return _switchView; }
            set
            {
                if (_switchView != value)
                {
                    _switchView = value;
                    OnPropertyChanged(nameof(SwitchView));
                }
            }
        }

        private int _switchView = 0;

        private Devices.IDevice[] _devices = null;
        private Devices.IDevice _selectedDevice = null;


        public Devices.IDevice[] Devices
        {
            get
            {
                return _devices;
            }
            set
            {
                _devices = value;
                OnPropertyChanged(nameof(Devices));
            }
        }

        public Devices.IDevice SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }

        public string InfoMessage
        {
            get
            {
               DataTable _results= _backofficedal.GetRequests(SelectedPrint.PrintBatchId);
               var result_reprint= _results.Select("printing_status ='" + (PrintingStatus.RE_PRINT+"'"));
                var result_printed = _results.Select("printing_status ='" + (PrintingStatus.PRINTED + "'"));
                var result_spoiled = _results.Select("printing_status ='" + (PrintingStatus.CARD_SPOILED + "'"));

                string spoiledcards="0", reprintcards ="0",printedcards="0";
                if(result_reprint != null)
                   reprintcards = result_reprint.ToArray().Length.ToString();
                if (result_spoiled != null)
                    spoiledcards = result_spoiled.ToArray().Length.ToString();
                if (result_printed != null)
                    printedcards = result_printed.ToArray().Length.ToString();
                return string.Format("Spoiled cards : {0},Re-printed Cards: {1},Printed cards: {2}",
                          spoiledcards,reprintcards, printedcards);

                
            }
        }
        //private string[] _deviceTypes = new string[]
        //    {
        //        "USB", "Network"
        //    };
        public string Uri { get { return Settings.Default.Uri; } }
        public ObservableCollection<KeyValuePair<int, string>> DeviceTypes { get; set; }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

       
        #endregion



    }
    

}
