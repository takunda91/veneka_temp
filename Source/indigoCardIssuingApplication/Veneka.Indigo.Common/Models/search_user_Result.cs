
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
    
public partial class search_user_Result
{

    public long id { get; set; }

    public byte[] username { get; set; }

    public byte[] first_name { get; set; }

    public byte[] last_name { get; set; }

    public byte[] employeeID { get; set; }

    public string password { get; set; }

    public int issuerID { get; set; }

    public string branchCode { get; set; }

    public Nullable<System.DateTime> last_login_date { get; set; }

    public Nullable<System.DateTime> last_login_attempt { get; set; }

    public Nullable<int> number_of_incorrect_logins { get; set; }

    public Nullable<System.DateTime> last_password_changed_date { get; set; }

    public string user_status { get; set; }

    public string user_group { get; set; }

    public string gender { get; set; }

    public Nullable<int> online { get; set; }

    public string workstation { get; set; }

    public int user_group_id { get; set; }

    public Nullable<int> branch_id { get; set; }

}

}
