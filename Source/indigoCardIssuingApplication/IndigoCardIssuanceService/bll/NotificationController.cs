using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.IssuerManagement.objects;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.IssuerManagement;

namespace IndigoCardIssuanceService.bll
{
    internal class NotificationController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NotificationController));
        private readonly NotificationsService _notificationservice = new NotificationsService();
        internal Response<long> InsertNotificationforBatch(NotificationMessages notifications, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_notificationservice.InsertNotificationforBatch(notifications, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<long>(1, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<long>(0, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long>(0,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<long> UpdateNotificationforBatch(NotificationMessages notifications, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_notificationservice.UpdateNotificationforBatch(notifications, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<long>(1, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<long>(0, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long>(0,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<List<notification_batchResult>> GetNotificationBatch(NotificationMessages messages, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<notification_batchResult>>(_notificationservice.GetNotificationBatch(messages,  auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<notification_batchResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<notification_batch_ListResult>> ListNotificationBatches(int issuerid,int pageIndex, int rowsperpage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<notification_batch_ListResult>>(_notificationservice.ListNotificationBatches(issuerid, pageIndex, rowsperpage),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<notification_batch_ListResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<bool> DeleteNotificationBatch( NotificationMessages messages, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<bool>(_notificationservice.DeleteNotificationBatch(messages, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        internal Response<long> InsertNotificationforBranch(NotificationMessages notifications, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_notificationservice.InsertNotificationforBranch(notifications, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<long>(1, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<long>(0, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long>(0,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<long> UpdateNotificationforBranch(NotificationMessages notifications, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_notificationservice.UpdateNotificationforBranch( notifications, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new Response<long>(1, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<long>(0, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<long>(0,
                                          ResponseType.ERROR,
                                          "Error when processing request.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<List<notification_branchResult>> GetNotificationBranch(NotificationMessages messages, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<notification_branchResult>>(_notificationservice.GetNotificationBranch(messages,auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<notification_branchResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<notification_branch_ListResult>> ListNotificationBraches(int issuerid, int pageIndex, int rowsperpage, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<notification_branch_ListResult>>(_notificationservice.ListNotificationBraches(issuerid, pageIndex, rowsperpage),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<notification_branch_ListResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<bool> DeleteNotificationBranch(NotificationMessages messages, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<bool>(_notificationservice.DeleteNotificationBranch(messages, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<bool>(false,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
    }
}
