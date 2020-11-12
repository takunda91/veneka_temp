using System;

namespace Veneka.Indigo.Common.Objects
{
    public class DatabaseResponse
    {
        public DBResponseMessage dbResponseMessage;
        public string responseMessage = "";
        public object responseObject = null;
        public bool responseSuccess = false;

        public DatabaseResponse(string strDBResponse)
        {
            dbResponseMessage = CommonGeneral.ValidateDBResponse(strDBResponse);

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
            dbResponseMessage = CommonGeneral.ValidateDBResponse(strDBResponse);

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