using System;
using indigoCardIssuingWeb.CardIssuanceService;
namespace indigoCardIssuingWeb.SearchParameters
{
    [Serializable]
    public class FileDetailsSearch : ISearchParameters
    {
        public int? FileLoadId { get; set; }
        public FileStatus? FileLoaderStatus { get; set; }
        public string FileName { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? IssuerId { get; set; }
        public int RowsPerPage { get; set; }
        public int PageIndex { get; set; }
        public bool IsSearch { get; set; }
        public ISearchParameters PreviousSearchParameters { get; set; }
        public FileDetailsSearch()
        {

        }

        public FileDetailsSearch(int? fileLoadId, int? issuerId, FileStatus? fileStatus, string fileName, DateTime? dateFrom, DateTime? dateTo, int pageIndex, int rowsPerPage)
        {
            this.FileLoadId = fileLoadId;
            this.FileLoaderStatus = fileStatus;
            this.FileName = fileName;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.IssuerId = issuerId;
            this.PageIndex = pageIndex;
            this.RowsPerPage = rowsPerPage;
        }
    }

    public class FileDetails
    {
        private readonly int _failedRecords;
        private readonly string _fileComments;
        private readonly string _fileLoadedDate;
        private readonly string _fileName;
        private readonly int _issuerID;
        private readonly string _status;
        private readonly int _successfulRecords;        

        public FileDetails(string fileName, string status, string fileLoadedDate, int issuerID,
                           int successRecords, int failedRecords, string fileComments)
        {
            _status = status;
            _fileLoadedDate = fileLoadedDate;
            _issuerID = issuerID;
            _fileName = fileName;
            _successfulRecords = successRecords;
            _failedRecords = failedRecords;
            _fileComments = fileComments;
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

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public string Status
        {
            get { return _status; }
        }

        public string FileLoadedDate
        {
            get { return _fileLoadedDate; }
        }

        public string FileComments
        {
            get { return _fileComments; }
        }
    }
}