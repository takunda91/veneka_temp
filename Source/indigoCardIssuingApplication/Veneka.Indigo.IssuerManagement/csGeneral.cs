namespace Veneka.Indigo.IssuerManagement
{
    public sealed class csGeneral
    {
        public static InstitutionStatus GetInstitutionStatus(string status)
        {
            InstitutionStatus instStatus;
            switch (status)
            {
                case "ACTIVE":
                    instStatus = InstitutionStatus.ACTIVE;
                    break;

                case "INACTIVE":
                    instStatus = InstitutionStatus.INACTIVE;
                    break;
                case "DELETED":
                    instStatus = InstitutionStatus.DELETED;
                    break;
                default:
                    instStatus = InstitutionStatus.INACTIVE;
                    break;
            }
            return instStatus;
        }
    }
}