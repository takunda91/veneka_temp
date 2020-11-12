using Veneka.Indigo.AuditManagement.dal;
using Veneka.Indigo.Common;

namespace Veneka.Indigo.AuditManagement
{
    public class AuditController
    {
        private static readonly AuditContolDal auditor = new AuditContolDal();
        private static readonly string DASH_STRING = "-";

        private static void LogUserAction(int userid, AuditActionType userAction, string description,
                                          string workstation, string dataBefore, string dataAfter, int issuerID)
        {
            //auditor.InsertAudit(userid, (int)userAction,userAction.ToString(), description, workstation, dataBefore, dataAfter,
            //                    issuerID);
        }

        public static void LogAuditEvent(int userid, AuditActionType userAction, string description,
                                          string workstation, string dataBefore, string dataAfter, int issuerID)
        {
            auditor.InsertAudit(userid, (int)userAction, userAction.ToString(), description, workstation, dataBefore, dataAfter, issuerID);
        }

        public static void addAudit(int userid, AuditActionType userAction, string description, string workstation,
                                    string dataBefore, string dataAfter, int issuerID)
        {
            LogUserAction(userid, userAction, description, workstation, dataBefore, dataAfter, issuerID);
        }

        public static void addAudit(int userid, AuditActionType userAction, string description, string workstation,
                                    string dataBefore, string dataAfter)
        {
            LogUserAction(userid, userAction, description, workstation, dataBefore, dataAfter, -1);
        }

        public static void addAudit(int userid, AuditActionType userAction, string description, string workstation)
        {
            LogUserAction(userid, userAction, description, workstation, DASH_STRING, DASH_STRING, -1);
        }

        public static void addAudit(int userid, AuditActionType userAction, string description, string workstation,
                                    int issuerID)
        {
            LogUserAction(userid, userAction, description, workstation, DASH_STRING, DASH_STRING, issuerID);
        }
    }
}