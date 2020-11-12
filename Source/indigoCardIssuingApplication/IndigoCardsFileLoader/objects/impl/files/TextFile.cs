using System;
using System.Collections.Generic;
using System.IO;
using IndigoFileLoader.objects.root;

namespace IndigoFileLoader.objects.impl.files
{
    internal class TextFile : ACardsFile
    {
        private readonly string _fileName;
        private bool _ignoreHeader;
        private bool _ignoreTrailer;

        public TextFile(string fileName, bool ignoreHeader, bool ignoreTrailer)
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
            var fs = new FileStream(_fileName, FileMode.Open);

            try
            {
                var sr = new StreamReader(fs);

                string header = sr.ReadLine(); //discard the header;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    //TODO: RAB EOF also contains number of records... sanity check records against this number
                    if (!line.StartsWith("EOF ")) //if this is not the end of file / footer
                    {
                        var record = new[] {line};                        
                        lines.Add(record);
                    }
                }                
            }
            finally
            {
                fs.Close();
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