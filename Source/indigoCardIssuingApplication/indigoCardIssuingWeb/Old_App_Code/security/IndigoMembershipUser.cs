using indigoCardIssuingWeb.CardIssuanceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace indigoCardIssuingWeb.Old_App_Code.security
{
    public class IndigoMembershipUser : MembershipUser
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDomainUser { get; set; }
        public bool ISMultiFactorenabled { get; set; }

        public int AuthConfigId { get; set; }

        public string Workstation { get; set; }
        public bool MustChangePassword { get; set; }

        public IndigoMembershipUser(decrypted_user user) :
            base("IndigoMembershipProvider", user.username, user.user_id, user.user_email, String.Empty, String.Empty,
                  //true, false, DateTime.Now, DateTime.Now, DateTime.Now, (System.DateTimeOffset)user.last_password_changed_date, DateTime.Now)
                  true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)

        {


            this.UserId = user.user_id;
            this.FirstName = user.first_name;
            this.LastName = user.last_name;

            //if (user.connection_parameter_id == null)
            //    this.IsDomainUser = false;
            //else
            //    this.IsDomainUser = true;

            this.Workstation = user.workstation;
        }


    }
}