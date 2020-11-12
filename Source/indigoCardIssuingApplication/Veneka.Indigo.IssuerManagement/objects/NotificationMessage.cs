using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.IssuerManagement.objects
{
    public class NotificationMessages
    {
        public List<NotificationMessage> messages { get; set; }
        public int channel_id { get; set; }
        public int issuerid { get; set; }
        public int distbatchtypeid { get; set; }
        public int distbatchstatusesid { get; set; }

        public int branchcardstatusesid { get; set; }
        public int cardissuemethodid { get; set; }
       


    }
    public class NotificationMessage
    {
        public int language_id { get; set; }
        public string language_name { get; set; }
        public string notification_text { get; set; }
        public string subject_text { get; set; }
        public string from_address { get; set; }

    }
}
