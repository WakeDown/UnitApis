//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class holiday_work_confirms
    {
        public int id { get; set; }
        public string employee_sid { get; set; }
        public Nullable<int> id_hw_document { get; set; }
        public System.DateTime dattim1 { get; set; }
        public string full_name { get; set; }
        public bool enabled { get; set; }
    }
}