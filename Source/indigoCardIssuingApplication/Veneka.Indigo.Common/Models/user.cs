
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
    using System.Collections.Generic;
    
public partial class user
{

    public long user_id { get; set; }

    public int user_status_id { get; set; }

    public int user_gender_id { get; set; }

    public byte[] username { get; set; }

    public byte[] first_name { get; set; }

    public byte[] last_name { get; set; }

    public byte[] password { get; set; }

    public string user_email { get; set; }

    public bool online { get; set; }

    public byte[] employee_id { get; set; }

    public Nullable<System.DateTimeOffset> last_login_date { get; set; }

    public Nullable<System.DateTimeOffset> last_login_attempt { get; set; }

    public Nullable<int> number_of_incorrect_logins { get; set; }

    public Nullable<System.DateTimeOffset> last_password_changed_date { get; set; }

    public string workstation { get; set; }

    public Nullable<int> language_id { get; set; }

    public byte[] username_index { get; set; }

    public byte[] instant_authorisation_pin { get; set; }

    public Nullable<System.DateTimeOffset> last_authorisation_pin_changed_date { get; set; }

    public string time_zone_utcoffset { get; set; }

    public string time_zone_id { get; set; }

    public Nullable<long> useradmin_user_id { get; set; }

    public Nullable<System.DateTimeOffset> record_datetime { get; set; }

    public Nullable<long> approved_user_id { get; set; }

    public Nullable<System.DateTimeOffset> approved_datetime { get; set; }

    public Nullable<int> authentication_configuration_id { get; set; }

}

}