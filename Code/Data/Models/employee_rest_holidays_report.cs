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
    
    public partial class employee_rest_holidays_report
    {
        public string department { get; set; }
        public string employee { get; set; }
        public Nullable<int> year { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public int duration { get; set; }
        public System.DateTime date_create { get; set; }
        public bool can_edit { get; set; }
        public bool confirmed { get; set; }
        public string confimator { get; set; }
    }
}
