using Common.Logging;
using IndigoCardIssuanceService.bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Veneka.Indigo.Integration.Common;
using Veneka.Module.ISO8583.WCF;

namespace IndigoCardIssuanceService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ISO8583" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ISO8583.svc or ISO8583.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = "http://schemas.veneka.com/Indigo")]
    public class ISO8583 : IISO8583
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ISO8583));
        private readonly TerminalManagementController _terminalController = new TerminalManagementController();

        public ISOMessage Send(ISOMessage message)
        {
            string deviceId = "Unknown";
            StringBuilder fieldDebug = new StringBuilder();

            ISOMessage response = new ISOMessage();
            response.Fields = new List<Field>();

            if (message == null || message.Fields == null)
            {
                log.Warn("Empty message received.");
                response.Fields.Add(new Field(39, null, "99"));
                return response;
            }

            try
            {                
                deviceId = message.Fields.Single(s => s.Fieldid == 41).Value;
                log.Trace(t => t("{0} : ISO Message recieved, MTI={1}{2}", deviceId, message.MessageClass, message.MessageFunction));
                if (log.IsDebugEnabled)
                {
                    foreach (var field in message.Fields.OrderBy(o => o.Fieldid))
                        fieldDebug.AppendLine("Field " + field.Fieldid.ToString("000") + "=" + field.Value);

                    log.Debug(d => d("{0} : ISO Message{1}{2}", deviceId, Environment.NewLine, fieldDebug.ToString().Trim()));
                }

                //Handshake - TPK
                if (message.MessageClass + message.MessageFunction == "0800")
                {
                    response.MessageClass = "08";
                    response.MessageFunction = "10";

                    log.Trace(t => t("{0} : Generate session key for device.", deviceId));
                    var resp = _terminalController.GetTerminalSessionKeyClear(deviceId, 0, -2, deviceId);

                    if (resp.ResponseType == ResponseType.SUCCESSFUL)
                    {
                        log.Trace(t => t("{0} : Session key generated for device.", deviceId));
                        log.Debug(d => d("{0} : Session key for device {1}.", deviceId, resp.Value.RandomKey));

                        response.Fields.Add(new Field(39, null, resp.ResponseException));
                        response.Fields.Add(new Field(62, null, resp.Value.RandomKey.Substring(resp.Value.RandomKey.Length % 16)));
                    }
                    else
                    {
                        log.Trace(t => t("{0} : Session key failed to generate for device.", deviceId));
                        log.WarnFormat("{0} : Failed to generate session key for device, error = {1}-{2}", deviceId, resp.ResponseException, resp.ResponseMessage);
                        response.Fields.Add(new Field(39, null, resp.ResponseException));
                    }
                }
                //PIN
                else if (message.MessageClass + message.MessageFunction == "0200")
                {
                    response.MessageClass = "02";
                    response.MessageFunction = "10";

                    log.Trace(t => t("{0} : Update PIN message from device ", deviceId));

                    var trackData = message.Fields.Single(s => s.Fieldid == 35).Value;
                    Track2 track2 = new Track2(trackData);

                    TerminalCardData cardData = new TerminalCardData
                    {
                        PINBlockFormat = 0,
                        PINBlock = message.Fields.Single(s => s.Fieldid == 57).Value,
                        IsTrack2Encrypted = false,
                        Track2 = trackData,
                        IsPANEncrypted = false,
                        PAN = track2.PAN
                    };

                    //PINResetISO8583(string deviceId, string operatorCode, string pinBlock, string trackData, long auditUserId, string auditWorkstation)
                    var resp = _terminalController.PINResetISO8583(
                        deviceId,
                        message.Fields.Single(s => s.Fieldid == 52).Value,
                        cardData,
                        //message.Fields.Single(s => s.Fieldid == 57).Value,
                        //message.Fields.Single(s => s.Fieldid == 35).Value,
                        -2, deviceId);

                    if (resp.ResponseType == ResponseType.SUCCESSFUL)
                    {
                        log.Trace(t => t("{0} : PIN updated successfully form device.", deviceId));
                        response.Fields.Add(new Field(39, null, resp.ResponseException));
                    }
                    else
                    {
                        log.Trace(t => t("{0} : Update PIN failed for device.", deviceId));
                        log.WarnFormat("{0} : Failed to update PIN for device, error = {1}-{2}", deviceId, resp.ResponseException, resp.ResponseMessage);

                        response.Fields.Add(new Field(39, null, resp.ResponseException));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(deviceId + " : Unexpected error.", ex);
                response.Fields.Add(new Field(39, null, "99"));
            }
            finally
            {
                //echo back fields:                
                foreach (var field in message.Fields)
                {
                    switch (field.Fieldid)
                    {
                        case 3:
                        case 11:
                        case 41:
                        case 42: response.Fields.Add(field); break;
                        default:
                            break;
                    }
                }

                if(log.IsDebugEnabled)
                {
                    fieldDebug.Clear();

                    foreach (var field in response.Fields.OrderBy(o => o.Fieldid))
                        fieldDebug.AppendLine("Field " + field.Fieldid.ToString("000") + "=" + field.Value);                            
                }
            }

            log.Trace(t => t(deviceId + " : Responding with MTI=" + response.MessageClass + response.MessageFunction + ", code=" + response.Fields.Single(s => s.Fieldid == 39).Value));
            log.Debug(d => d(deviceId + " : ISO Response " + Environment.NewLine + fieldDebug.ToString().Trim()));

            return response;
        }
    }
}
