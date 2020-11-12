using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EProcess.Indigo._2._0._0.Migration.Common.Models
{
    /// <summary>
    /// Class that used to represent a user in a report
    /// </summary>
    public class User
    {
        public long user_id { get; set; }
        public string username { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string user_status_value { get; set; }
        public string user_gender_value { get; set; }
        public string user_email { get; set; }
        public DateTime? last_login_date { get; set; }
        public DateTime? last_login_attempt { get; set; }
        public string workstation { get; set; }
    }
}
