
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
    
public partial class issuer_interface
{

    public int interface_type_id { get; set; }

    public int issuer_id { get; set; }

    public int connection_parameter_id { get; set; }

    public int interface_area { get; set; }

    public string interface_guid { get; set; }



    public virtual issuer issuer { get; set; }

    public virtual interface_type interface_type { get; set; }

}

}
