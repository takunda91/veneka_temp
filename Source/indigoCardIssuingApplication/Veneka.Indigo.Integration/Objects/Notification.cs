using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Veneka.Indigo.Integration.Objects
{
    public enum NotificationType { Branch, Batch };

    public class Notification
    {
        private readonly string[] _placeHolders = new string[]
        {
            "<CardRef>", "<Title>", "<LastName>", "<FirstName>", "<BatchListURL>", "<BatchReference>"
        };

        private readonly Guid _messageId;
        private readonly string _address = String.Empty;
        private readonly string _subject = String.Empty;
        private readonly string _text = String.Empty;
        private readonly string _fromaddress = String.Empty;
        private readonly int _channel_id = 0;

        private readonly int _issuerId;
        private readonly int _branch_id;
        private readonly int _dist_batch_type_id;


        #region Constructors
        public Notification(Guid messageId, string address,string fromaddress, string subject, string text)
        {
            this._messageId = messageId;
            this._address = address;
            this._subject = subject;
            this._text = text;
            this._fromaddress = fromaddress;
        }

        public Notification(DataRow dataRow, NotificationType notificationType)
        {
            if (notificationType == NotificationType.Branch)
            {
                this._issuerId = dataRow.Field<int>("issuer_id");
                this._messageId = dataRow.Field<Guid>("branch_message_id");
                this._channel_id = dataRow.Field<int>("channel_id");
                this._address = dataRow.Field<string>("contact_number");
                this._subject = dataRow.Field<string>("subject_text");
                this._text = BuildBranchMessage(dataRow); 
                this._fromaddress = dataRow.Field<string>("from_address");

            }
            else if(notificationType == NotificationType.Batch)
            {
                this._issuerId = dataRow.Field<int>("issuer_id");
                this._messageId = dataRow.Field<Guid>("batch_message_id");
                this._dist_batch_type_id = dataRow.Field<int>("dist_batch_type_id");
                this._branch_id = dataRow.Field<int>("branch_id");
                this._channel_id = dataRow.Field<int>("channel_id");
                this._address = dataRow.Field<string>("contact_number");
                this._subject = dataRow.Field<string>("subject_text");
                this._text = BuildBranchMessage(dataRow);
                this._fromaddress = dataRow.Field<string>("from_address");

            }
            else            
                throw new ArgumentException("Unknown notification type.", "notificationType");            
        }

        public Notification(DataRow dataRow, DataRow userRow, NotificationType notificationType)
        {
            if (notificationType == NotificationType.Branch)
            {
                this._issuerId = dataRow.Field<int>("issuer_id");
                this._messageId = dataRow.Field<Guid>("branch_message_id");
                this._channel_id = dataRow.Field<int>("channel_id");
                this._address = dataRow.Field<string>("contact_number");
                this._subject = dataRow.Field<string>("subject_text");
                this._text = BuildBranchMessage(dataRow);
                this._fromaddress = dataRow.Field<string>("from_address");

            }
            else if (notificationType == NotificationType.Batch)
            {
                this._issuerId = dataRow.Field<int>("issuer_id");
                this._messageId = dataRow.Field<Guid>("batch_message_id");
                this._dist_batch_type_id = dataRow.Field<int>("dist_batch_type_id");
                this._channel_id = dataRow.Field<int>("channel_id");
                this._branch_id = dataRow.Field<int>("branch_id");
                this._address = userRow.Field<string>("user_email");
                this._subject = dataRow.Field<string>("subject_text");
                this._text = BuildBatchMessage(dataRow, userRow);
                this._fromaddress = dataRow.Field<string>("from_address");

            }
            else
                throw new ArgumentException("Unknown notification type.", "notificationType");
        }
        #endregion

        private string BuildBranchMessage(DataRow dataRow)
        {
            string rtnText = dataRow.Field<string>("notification_text");
            
            foreach (var placeHolder in _placeHolders)
            {
                switch(placeHolder)
                {
                    case "<CardRef>":
                        rtnText = Regex.Replace(rtnText, placeHolder, dataRow.Field<string>("card_request_reference") ?? "", RegexOptions.IgnoreCase);
                        //rtnText.Replace(placeHolder, dataRow.Field<string>("card_request_reference"));
                        break;
                    case "<Title>":
                        rtnText = Regex.Replace(rtnText, placeHolder, dataRow.Field<string>("customer_title_name") ?? "", RegexOptions.IgnoreCase);
                        //rtnText.Replace(placeHolder, dataRow.Field<string>("customer_title_name"));
                        break;
                    case "<LastName>":
                        rtnText = Regex.Replace(rtnText, placeHolder, dataRow.Field<string>("customer_last_name") ?? "", RegexOptions.IgnoreCase);
                        //rtnText.Replace(placeHolder, dataRow.Field<string>("customer_last_name"));
                        break;
                    case "<FirstName>":
                        rtnText = Regex.Replace(rtnText, placeHolder, dataRow.Field<string>("customer_first_name") ?? "", RegexOptions.IgnoreCase);
                        //rtnText.Replace(placeHolder, dataRow.Field<string>("customer_first_name"));
                        break;
                    default:
                        break;
                }                
            }

            //[notification_branch_outbox].branch_message_id 
            //, [cards].card_request_reference
            //, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].Id_number)) as 'id_number'
            //, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].contact_number)) as 'contact_number'
            //, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_first_name)) as 'customer_first_name'
            //, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_middle_name)) as 'customer_middle_name'
            //, CONVERT(VARCHAR(MAX),DECRYPTBYKEY([customer_account].customer_last_name)) as 'customer_last_name'
            //, [customer_title_language].language_text AS 'customer_title_name'
            //,[notification_branch_messages].notification_text
            //, [notification_branch_messages].subject_text

            return rtnText;
        }

        private string BuildBatchMessage(DataRow dataRow, DataRow userRow)
        {
            string rtnText = dataRow.Field<string>("notification_text");

            foreach (var placeHolder in _placeHolders)
            {
                switch (placeHolder)
                {
                    case "<LastName>":
                        rtnText = Regex.Replace(rtnText, placeHolder, userRow.Field<string>("last_name") ?? "", RegexOptions.IgnoreCase);
                        //rtnText.Replace(placeHolder, dataRow.Field<string>("customer_last_name"));
                        break;
                    case "<FirstName>":
                        rtnText = Regex.Replace(rtnText, placeHolder, userRow.Field<string>("first_name") ?? "", RegexOptions.IgnoreCase);
                        //rtnText.Replace(placeHolder, dataRow.Field<string>("customer_first_name"));
                        break;
                    case "<BatchReference>":
                        rtnText = Regex.Replace(rtnText, placeHolder, dataRow.Field<string>("dist_batch_reference") ?? "", RegexOptions.IgnoreCase);
                        break;
                    case "<BatchListURL>":
                        //webpages/dist/DistBatchList.aspx?status=0&distBatchTypeId=1
                        rtnText = Regex.Replace(rtnText, placeHolder, dataRow.Field<string>("dist_batch_status_name") ?? "", RegexOptions.IgnoreCase);
                        break;
                    default:
                        break;
                }
            }

            return rtnText;
        }

        #region Properties
        public Guid MessageId { get { return _messageId; } }
        public string Address { get { return _address; } }
        public string Subject {  get { return _subject; } }
        public string FromAddress { get { return _fromaddress; } }
        public int Channel { get { return _channel_id; } }

        public string Text { get { return _text; } }
        public int IssuerId { get { return _issuerId; } }
        public DateTime Sent { get; set; }
        public bool IsSetn { get; set; }
        #endregion
    }
}
