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
    
    public partial class get_document_list_for_employee_Result
    {
        public int id { get; set; }
        public string NAME { get; set; }
        public System.Guid data_sid { get; set; }
        public Nullable<System.DateTime> meet_date { get; set; }
        public Nullable<System.DateTime> open_date { get; set; }
        public Nullable<int> folder_id { get; set; }
        public string folder_name { get; set; }
        public System.DateTime create_date { get; set; }
        public Nullable<int> folder_parent_id { get; set; }
    }
}
