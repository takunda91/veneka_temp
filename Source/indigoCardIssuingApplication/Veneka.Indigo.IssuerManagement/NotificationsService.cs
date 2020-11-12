using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.IssuerManagement.objects;
using Veneka.Indigo.IssuerManagement.dal;

namespace Veneka.Indigo.IssuerManagement
{
    public class NotificationsService
    {
        private readonly ResponseTranslator _translator = new ResponseTranslator();
        private readonly NotificationDAL _notificatinDAL = new NotificationDAL();

      

        #region"BATCH NOTIFICATION CURD OPERATIONS "

        public bool InsertNotificationforBatch(NotificationMessages notifications, int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _notificatinDAL.InsertNotificationforBatch(notifications, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }




        public bool UpdateNotificationforBatch( NotificationMessages notifications,int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _notificatinDAL.UpdateNotificationforBatch(notifications, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public List<notification_batchResult> GetNotificationBatch(NotificationMessages notifications, long auditUserId, string auditWorkstation)
        {

            return _notificatinDAL.GetNotificationBatch(notifications, auditUserId, auditWorkstation);

        }

        public List<notification_batch_ListResult> ListNotificationBatches(int issuerid, int pageIndex, int rowsperpage)
        {
            return _notificatinDAL.ListNotificationBatches(issuerid, pageIndex, rowsperpage);

        }
        public bool DeleteNotificationBatch(NotificationMessages notifications, long auditUserId, string auditWorkstation)
        {
             _notificatinDAL.DeleteNotificationBatch(notifications, auditUserId, auditWorkstation);
            return true;
        }
        #endregion

        #region"BRANCH NOTIFICATION CURD OPERATIONS "

        public bool InsertNotificationforBranch(NotificationMessages notifications,int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _notificatinDAL.InsertNotificationforBranch(notifications, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public bool UpdateNotificationforBranch(NotificationMessages notifications,int languageId, long auditUserId, string auditWorkstation, out string responseMessage)
        {
            var response = _notificatinDAL.UpdateNotificationforBranch(notifications, auditUserId, auditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, auditUserId, auditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }

        public List<notification_branchResult> GetNotificationBranch(NotificationMessages notifications, long auditUserId, string auditWorkstation)
        {
            return _notificatinDAL.GetNotificationBranch(notifications, auditUserId,auditWorkstation);

        }

        public List<notification_branch_ListResult> ListNotificationBraches(int issuerid, int pageIndex, int rowsperpage)
        {
            return _notificatinDAL.ListNotificationBraches(issuerid, pageIndex, rowsperpage);

        }
        public bool DeleteNotificationBranch(NotificationMessages notifications,long auditUserId, string auditWorkstation)
        {
            _notificatinDAL.DeleteNotificationBranch(notifications, auditUserId, auditWorkstation);
            return true;
        }
        #endregion
    }
}
