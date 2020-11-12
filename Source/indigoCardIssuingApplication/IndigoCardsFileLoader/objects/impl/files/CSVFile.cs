using System;
using System.Collections.Generic;
using System.IO;
using IndigoFileLoader.objects.root;

namespace IndigoFileLoader.objects.impl.files
{
    internal class CSVFile : ACardsFile
    {
        private readonly string _fileName;
        private readonly bool _ignoreHeader;
        private readonly bool _ignoreTrailer;

        public CSVFile(string fileName, bool ignoreHeader, bool ignoreTrailer)
        {
            _ignoreHeader = ignoreHeader;
            _ignoreTrailer = ignoreTrailer;
            _fileName = fileName;
        }

        public override FileExtensionType CardsFileType
        {
            get { return FileExtensionType.CSV; }
        }

        public override List<BatchRecord> GetBatchRecords()
        {
            return new List<BatchRecord>();
        }

        public override List<string[]> ReadFile()
        {
            var lines = new List<string[]>();
            int recordsRead = 0;
            string lineInError = "";
            try
            {
                using (var fileReader = new StreamReader(_fileName))
                {
                    string lineRead = fileReader.ReadLine(); //read off the header

                    if (_ignoreHeader)
                        lineRead = fileReader.ReadLine();

                    while (lineRead != null)
                    {
                        recordsRead++;
                        string[] row = lineRead.Replace("\"", "").Split(',');
                        lineInError = lineRead;
                        lines.Add(row);
                        lineRead = fileReader.ReadLine();
                    }

                    if (_ignoreTrailer && recordsRead > 0)
                        lines.RemoveAt(recordsRead - 1);
                }
            }
            catch //(Exception e)
            {
                lines.Clear();
            }


            return lines;
        }

        public override List<AFileRecord> GetFileRecords()
        {
            return new List<AFileRecord>();
        }

        public override bool IsHeaderRecord(AFileRecord record)
        {
            return false;
        }

        public override bool IsDetailedRecord(AFileRecord record)
        {
            return false;
        }

        public override bool IsTrailerRecord(AFileRecord record)
        {
            return false;
        }
    }
}