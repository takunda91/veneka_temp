
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
    
public partial class ConnectionParamsResult
{

    public int connection_parameter_id { get; set; }

    public string connection_name { get; set; }

    public string address { get; set; }

    public int port { get; set; }

    public string path { get; set; }

    public Nullable<int> protocol { get; set; }

    public int auth_type { get; set; }

    public string username { get; set; }

    public string password { get; set; }

    public Nullable<int> header_length { get; set; }

    public string identification { get; set; }

    public int connection_parameter_type_id { get; set; }

    public Nullable<int> timeout_milli { get; set; }

    public Nullable<int> buffer_size { get; set; }

    public string doc_type { get; set; }

    public string name_of_file { get; set; }

    public Nullable<bool> file_delete_YN { get; set; }

    public Nullable<int> file_encryption_type_id { get; set; }

    public Nullable<bool> duplicate_file_check_YN { get; set; }

    public string private_key { get; set; }

    public string public_key { get; set; }

    public string domain_name { get; set; }

    public Nullable<bool> is_external_auth { get; set; }

    public Nullable<int> remote_port { get; set; }

    public string remote_username { get; set; }

    public string remote_password { get; set; }

    public string nonce { get; set; }

}

}
