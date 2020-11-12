using System;

namespace IndigoFileLoader.objects
{
    public class FileDetails
    {
        private readonly int _failedRecords;
        private readonly string _fileComments;
        private readonly string _fileCreatedDate;
        private readonly String _fileDirectory;
        private readonly long _fileHistoryID;
        private readonly string _fileLoadedDate;
        private readonly string _fileName;
        private readonly FileType _fileType;
        private readonly int _issuerID;
        private readonly long _size;
        private readonly String _status;
        private readonly int _successfulRecords;
        //private String _issuerName;

        public FileDetails(long fileHistoryID, string fileName, FileType fileType, string fileCreatedDate,
                           String fileDirectory, long size, String status, string fileLoadedDate,
                           int issuerID, int successRecords, int failedRecords, string fileComments)
        {
            _fileHistoryID = fileHistoryID;
            _fileCreatedDate = fileCreatedDate;
            _fileDirectory = fileDirectory;
            _size = size;
            _status = status;
            _fileLoadedDate = fileLoadedDate;
            _issuerID = issuerID;
            _fileName = fileName;
            _successfulRecords = successRecords;
            _failedRecords = failedRecords;
            _fileType = fileType;
            _fileComments = fileComments;
        }

        public long FileHistoryID
        {
            get { return _fileHistoryID; }
        }

        public int NumberOfSuccessfulRecords
        {
            get { return _successfulRecords; }
        }

        public int NumberOfFailedRecords
        {
            get { return _failedRecords; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public string FileCreatedDate
        {
            get { return _fileCreatedDate; }
        }

        public String FileDirectory
        {
            get { return _fileDirectory; }
        }

        //public String IssuerName
        //{
        //    get { return _issuerName; }
        //}

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public long Size
        {
            get { return _size; }
        }

        public String Status
        {
            get { return _status; }
        }

        public string FileLoadedDate
        {
            get { return _fileLoadedDate; }
        }

        public FileType FileType
        {
            get { return _fileType; }
        }

        public string FileComments
        {
            get { return _fileComments; }
        }
    }
}