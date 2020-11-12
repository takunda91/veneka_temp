using System;
using Veneka.Indigo.Common;

namespace Veneka.Indigo.CardManagement.objects
{
    public class DatabaseResponse
    {
        public DBResponseMessage dbResponseMessage;
        public string responseMessage = "";
        public object responseObject = null;
        public bool responseSuccess = false;

        public DatabaseResponse() { }//serialization param-less .ctor

        public DatabaseResponse(string strDBResponse)
        {
            dbResponseMessage = csGeneral.ValidateDBResponse(strDBResponse);

            if (dbResponseMessage.Equals(DBResponseMessage.SUCCESS))
                responseSuccess = true;
        }

        public DatabaseResponse(DBResponseMessage dbResp, string strResponseMessage)
        {
            if (dbResp.Equals(DBResponseMessage.SUCCESS))
                responseSuccess = true;
            dbResponseMessage = dbResp;
            responseMessage = strResponseMessage;
        }

        public DatabaseResponse(DBResponseMessage dbResp, string strResponseMessage, object dataObject)
        {
            if (dbResp.Equals(DBResponseMessage.SUCCESS))
                responseSuccess = true;
            dbResponseMessage = dbResp;
            responseMessage = strResponseMessage;
            responseObject = dataObject;
        }

        public DatabaseResponse(string strDBResponse, string strResponseMessage, object dataObject)
        {
            dbResponseMessage = csGeneral.ValidateDBResponse(strDBResponse);

            if (dbResponseMessage.Equals(DBResponseMessage.SUCCESS))
                responseSuccess = true;

            responseMessage = strResponseMessage;
            responseObject = dataObject;
        }


        public DatabaseResponse(Exception ex)
        {
            dbResponseMessage = DBResponseMessage.SYSTEM_ERROR;
            responseMessage = ex.Message;
        }
    }
}