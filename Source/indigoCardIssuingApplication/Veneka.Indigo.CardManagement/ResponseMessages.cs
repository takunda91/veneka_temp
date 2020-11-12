namespace Veneka.Indigo.CardManagement
{
    public enum ResponseMessage
    {
        SUCCESS,
        INVALID_STATUS,
        FAILED
    }

    internal class ResponseMessages
    {
        public ResponseMessage validateSQLResposeMessage(string sqlResponseMessage)
        {
            sqlResponseMessage = sqlResponseMessage.Trim();

            if (sqlResponseMessage.Equals(ResponseMessage.SUCCESS.ToString()))
                return ResponseMessage.SUCCESS;

            return ResponseMessage.FAILED;
        }
    }
}