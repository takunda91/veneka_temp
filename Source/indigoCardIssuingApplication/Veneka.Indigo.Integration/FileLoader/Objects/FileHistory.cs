using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    public class FileHistory
    {
        private string loadBatchReference = DateTimeOffset.Now.ToString("yyyyMMddhhmmss");

        public long? FileId { get; set; }
        public int? IssuerId { get; set; }
        public int FileStatusId
        {
            get
            {
                return (int)FileStatus;
            }
        }
        public FileStatuses FileStatus { get; set; }
        public int FileTypeId { get; set; }
        public int FileLoadId { get; set; }
        public string NameOfFile { get; set; }
        public DateTimeOffset FileCreatedDate { get; set; }
        public int FileSize { get; set; }
        public DateTimeOffset LoadDate { get; set; }
        public string FileDirectory { get; set; }
        public int? NumberSuccessfulRecords { get; set; }
        public int? NumberFailedRecords { get; set; }
        public string FileLoadComments { get; set; }
        public string LoadBatchReference 
        { 
            get
            {
                return loadBatchReference;
            }
            set
            {
                loadBatchReference = value;
            }
        }
    }
}
