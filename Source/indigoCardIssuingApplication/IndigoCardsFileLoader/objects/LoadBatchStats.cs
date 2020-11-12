using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using IndigoFileLoader.objects;

namespace IndigoFileLoader.objects
{
    public class LoadBatchStats
    {
        public List<FileInfo> SuccessFileload { get; set; }
        public List<FileInfo> PartialFileload { get; set; }
        public List<FileInfo> FailedFileload { get; set; }

        public List<FileInfo> DuplicateFile { get; set; }
        public List<FileInfo> InvalidFormatFile { get; set; }

        public List<CardFileRecord> TotalRecords { get; set; }
        public List<CardFileRecord> DuplicateRecordsInFile { get; set; }
        public List<CardFileRecord> DuplicateRecordsInDB { get; set; }
        public List<CardFileRecord> UnlicensedRecords { get; set; }

        public List<string> Comments { get; set; }

        public long FileLoadMilli { get; set; }
        public long FileReadMilli { get; set; }
        public long FileDbLoadMilli { get; set; }
    }
}
