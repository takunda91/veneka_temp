
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Veneka.Indigo.Common.Models
{

using System;
    
public partial class usp_get_user_pending_by_user_id_Result
{

    public long user_id { get; set; }

    public string username { get; set; }

    public string first_name { get; set; }

    public string last_name { get; set; }

    public string empoyee_id { get; set; }

    public string user_email { get; set; }

    public int user_status_id { get; set; }

    public Nullable<int> connection_parameter_id { get; set; }

    public Nullable<int> language_id { get; set; }

    public bool online { get; set; }

    public string workstation { get; set; }

    public Nullable<System.DateTime> last_password_changed_date { get; set; }

    public Nullable<int> number_of_incorrect_logins { get; set; }

    public Nullable<System.DateTimeOffset> last_login_attempt { get; set; }

    public string external_interface_id { get; set; }

    public string time_zone_Id { get; set; }

    public string time_zone_utcoffset { get; set; }

}

}
