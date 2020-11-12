using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.FileLoader.Objects;

namespace Veneka.Indigo.Integration.FileLoader.Validation
{
    public abstract class Validation
    {
        public virtual FileStatuses ValidateCardFile(CardFile cardFile, List<FileCommentsObject> fileComments, long auditUserId, string auditWorkstation)
        {
            FileStatuses fileStatus = FileStatuses.READ;
            foreach (var record in cardFile.CardFileRecords)
            {
                string comment;
                fileStatus = Validate(record, auditUserId, auditWorkstation, out comment);

                if (fileStatus != FileStatuses.READ)
                {
                    fileComments.Add(new FileCommentsObject(comment));
                    return fileStatus;
                }
            }

            return fileStatus;
        }

        public FileStatuses ValidateBulkRequestsFile(BulkRequestsFile bulkRequestsFile, List<FileCommentsObject> fileComments, long auditUserId, string auditWorkstation)
        {
            FileStatuses fileStatus = FileStatuses.READ;
            foreach (var record in bulkRequestsFile.CardRequestFileRecords)
            {
                string comment;
                fileStatus = Validate(record, auditUserId, auditWorkstation, out comment);

                if (fileStatus != FileStatuses.READ)
                {
                    fileComments.Add(new FileCommentsObject(comment));
                    return fileStatus;
                }
            }

            return fileStatus;
        }

        public abstract void Clear();

        public abstract FileStatuses Validate(CardFileRecord record, long auditUserId, string auditWorkstation, out string fileComment);
        
        public abstract FileStatuses Validate(BulkRequestRecord record, long auditUserId, string auditWorkstation, out string fileComment);
    }
}
