using IndigoDesktopApp.Device.Printer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Simulator
{
    public class PrintSimulator : IPrinter
    {
        private bool _hasEncoder = false;

        public PrintSimulator(string name, string manufacturer, string model, string firmwareVersion, int comPort, string deviceId, bool hasEncoder)
        {
            Name = name;
            Manufacturer = manufacturer;
            Model = model;
            FirmwareVersion = firmwareVersion;
            ComPort = comPort;
            DeviceId = deviceId;
            _hasEncoder = hasEncoder;
        }

        #region Properties
        public string Name { get; private set; }

        public string Manufacturer { get; private set; }

        public string Model { get; private set; }

        public string FirmwareVersion { get; private set; }

        public int ComPort { get; private set; }

        public string DeviceId { get; private set; }
        #endregion

        public event DeviceNotificationEventHandler OnDeviceNotifcation;

        protected virtual void RaiseDeviceNotification(string message, bool isCritical, EventArgs e)
        {
            OnDeviceNotifcation?.Invoke(this, message, isCritical, e);
        }

        public short Cancel()
        {
            RaiseDeviceNotification("Call to Cancel()", false, EventArgs.Empty);
            return PrinterCodes.Success;
        }

        public short Connect()
        {
            RaiseDeviceNotification("Call to Connect()", false, EventArgs.Empty);
            return PrinterCodes.Success;
        }

        public short Disconnect()
        {
            RaiseDeviceNotification("Call to Disconnect()", false, EventArgs.Empty);
            return PrinterCodes.Success;
        }

        public Dictionary<string, string> GetPrinterInfo()
        {
            return new Dictionary<string, string>()
            {
                { "Ribbbon Type", "Black" },
                { "Number of prints remaining", "20"},
                { "Prints until next clean", "230" }
            };
        }

        public IPrinterDetailFactory PrinterDetailFactory()
        {
            return new PrintSimDetailFactory();
        }

        public bool HasMagEncoder()
        {
            RaiseDeviceNotification("Call to HasMagEncoder()", false, EventArgs.Empty);
            return _hasEncoder;
        }

        public short Print(string productBin, ICardPrintDetails cardPrintDetails)
        {
            RaiseDeviceNotification("Call to Print()", false, EventArgs.Empty);

            System.Threading.Thread.Sleep(3000);

            //var pollTask = PollJobStatusAsync();            

            ExtractDetails(productBin, cardPrintDetails);
            //pollTask.Wait();

            return PrinterCodes.Success;
        }

        public short ReadAndPrint(string productBin, ICardPrintDetails cardPrintDetails, out IDeviceMagData magData)
        {
            RaiseDeviceNotification("Call to ReadAndPrint()", false, EventArgs.Empty);
            magData = null;
            System.Threading.Thread.Sleep(3000);
            ExtractDetails(productBin, cardPrintDetails);

            return PrinterCodes.Success;
        }

        private void ExtractDetails(string productBin, ICardPrintDetails cardPrintDetails)
        {
            var info = new StringBuilder();
            info.AppendLine("ProductBIN: " + productBin);

            foreach (var detail in cardPrintDetails.FrontPanelText)
            {
                info.AppendLine("Front Text Panel - " + detail.Text);
            }

            foreach (var detail in cardPrintDetails.BackPanelText)
            {
                info.AppendLine("Back Text Panel - " + detail.Text);
            }

            info.AppendLine("Front Image Count - " + cardPrintDetails.FrontPanelImages.Length);
            info.AppendLine("Back Image Count - " + cardPrintDetails.BackPanelImages.Length);

            RaiseDeviceNotification(info.ToString(), false, EventArgs.Empty);

            System.Threading.Thread.Sleep(5000);
        }

        public short PrintJobStatus()
        {
            throw new NotImplementedException();
        }        

        public IDeviceMagData ReadMagStripe()
        {
            RaiseDeviceNotification("Call to ReadMagStripe()", false, EventArgs.Empty);
            return null;
        }

        public void SetDeviceSettings(Dictionary<string, string> settings)
        {
            RaiseDeviceNotification("Call to SetDeviceSettings()", false, EventArgs.Empty);
        }

        private async Task<int> PollJobStatusAsync()
        {
            int i = 5;

            for(; i > 0; i--)
            {
                RaiseDeviceNotification("Polling Count " + i, false, EventArgs.Empty);
                await Task.Delay(5000);
            }

            return i;            
        }

            #region PrinterList
            public static PrintSimulator[] GetPrinterList()
        {
            return new PrintSimulator[]
            {
                new PrintSimulator("PrintSim+Encododer", "Veneka", "Simulator 123", "v1.0.0", 5, "abcd1234", true),
                new PrintSimulator("PrintSim", "Veneka", "Simulator 111", "v1.0.0", 6, "efg5678", false)
            };
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PrintSimulator() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
