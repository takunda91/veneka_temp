using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Common
{
    public class FileSequenceGenerator : ISequenceGenerator
    {
        private const string DateFormat = "yyyyMMdd";

        private readonly DirectoryInfo _seqDir;
        private Dictionary<string, FileStream> _fileStreams = new Dictionary<string, FileStream>();
        private readonly byte[] _buffer = new byte[18];
        private readonly object _lock = new object();

        public FileSequenceGenerator(DirectoryInfo seqenceDir)
        {
            if (!seqenceDir.Exists)
                seqenceDir.Create();

            _seqDir = seqenceDir;
        }

        private FileStream FetchStream(string sequenceName)
        {
            if (!_fileStreams.ContainsKey(sequenceName))            
                _fileStreams.Add(sequenceName, new FileStream(Path.Combine(_seqDir.FullName, sequenceName + ".seq"), FileMode.OpenOrCreate));

            return _fileStreams[sequenceName];
        }

        private int DailyReset(ref DateTime seqDate, int startIndex)
        {
            if (seqDate.DayOfYear != DateTime.Now.DayOfYear && seqDate.Year == DateTime.Now.Year)
            {
                seqDate = DateTime.Now;
                return 0;
            }
            else if (seqDate.Year != DateTime.Now.Year)
            {
                seqDate = DateTime.Now;
                return 0;
            }
            else
                return BitConverter.ToInt32(_buffer, startIndex);
        }

        private int WeeklyReset(ref DateTime seqDate, int startIndex)
        {
            throw new NotImplementedException();
        }


        private int MonthlyReset(ref DateTime seqDate, int startIndex)
        {
            if (seqDate.Month != DateTime.Now.Month)
            {
                seqDate = DateTime.Now;
                return 0;
            }
            else
                return BitConverter.ToInt32(_buffer, startIndex);
        }


        private int YearlyReset(ref DateTime seqDate, int startIndex)
        {
            if (seqDate.Year != DateTime.Now.Year)
            {
                seqDate = DateTime.Now;
                return 0;
            }
            else
                return BitConverter.ToInt32(_buffer, startIndex);
        }

        public int NextSequenceNumber(string sequenceName, ResetPeriod resetPeriod)
        {
            int currentSeq = 0;
            int seqIndex = 8;

            lock (_lock)
            {
                try
                {
                    FileStream stream = FetchStream(sequenceName);

                    stream.Position = 0;
                    stream.Read(_buffer, 0, 12);

                    DateTime seqDate;
                    if (!DateTime.TryParseExact(Encoding.ASCII.GetString(_buffer, 0, seqIndex), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out seqDate))
                        seqDate = DateTime.Now;

                    switch (resetPeriod)
                    {
                        case ResetPeriod.DAILY:
                            currentSeq = DailyReset(ref seqDate, seqIndex);
                            break;
                        case ResetPeriod.WEEKLY:
                            currentSeq = WeeklyReset(ref seqDate, seqIndex);
                            break;
                        case ResetPeriod.MONTHLY:
                            currentSeq = MonthlyReset(ref seqDate, seqIndex);
                            break;
                        case ResetPeriod.YEARLY:
                            currentSeq = YearlyReset(ref seqDate, seqIndex);
                            break;
                        case ResetPeriod.NONE:
                        default:
                            currentSeq = BitConverter.ToInt32(_buffer, seqIndex);
                            break;
                    }

                    currentSeq++;

                    stream.Position = 0;
                    stream.Write(Encoding.ASCII.GetBytes(seqDate.ToString(DateFormat)), 0, seqIndex);
                    stream.Position = seqIndex;
                    stream.Write(BitConverter.GetBytes(currentSeq), 0, 4);
                    stream.Flush();
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }

            return currentSeq;
        }


        public long NextSequenceNumberLong(string sequenceName, ResetPeriod resetPeriod)
        {
            throw new NotImplementedException();
        }

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
                foreach (var stream in _fileStreams.Values)
                    stream.Dispose();

                _fileStreams.Clear();


                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FileSequenceGenerator() {
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
