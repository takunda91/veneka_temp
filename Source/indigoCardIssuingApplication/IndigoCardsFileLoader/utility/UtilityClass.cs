using System;

namespace IndigoFileLoader.utility
{
    public sealed class UtilityClass
    {
        public static IssuerStatus GetInstitutionStatus(string status)
        {
            IssuerStatus instStatus;
            switch (status)
            {
                case "ACTIVE":
                    instStatus = IssuerStatus.ACTIVE;
                    break;

                case "INACTIVE":
                    instStatus = IssuerStatus.INACTIVE;
                    break;
                case " SUSPENDED":
                    instStatus = IssuerStatus.SUSPENDED;
                    break;
                case " DELETED":
                    instStatus = IssuerStatus.DELETED;
                    break;
                default:
                    instStatus = IssuerStatus.UNKNOWN;
                    break;
            }
            return instStatus;
        }

        public static FileStatus GetCardsFileStatus(string status)
        {
            foreach (FileStatus cardSatus in Enum.GetValues(typeof (FileStatus)))
            {
                if (cardSatus.ToString().Equals(status))
                    return cardSatus;
            }

            return FileStatus.FILE_CORRUPT;
        }

        public static FileType GetFileType(string type)
        {
            FileType fileType;
            switch (type)
            {
                case "CARD_IMPORT":
                    fileType = FileType.CARD_IMPORT;
                    break;

                case "PIN_MAILER":
                    fileType = FileType.PIN_MAILER;
                    break;
                default:
                    fileType = FileType.UNKNOWN;
                    break;
            }
            return fileType;
        }

        public static string HandleUnexpectedCharacters(string str)
        {
            //purpose to handle nasties. this should be ideally replaced by stored proc 
            str = str.Replace("'", "''");
            return str;
        }
    }
}