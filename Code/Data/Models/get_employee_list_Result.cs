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
    
    public partial class get_employee_list_Result
    {
        public int id { get; set; }
        public string ad_sid { get; set; }
        public int id_manager { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public string full_name { get; set; }
        public string display_name { get; set; }
        public int id_position { get; set; }
        public int id_organization { get; set; }
        public string email { get; set; }
        public string work_num { get; set; }
        public string mobil_num { get; set; }
        public short id_emp_state { get; set; }
        public int id_department { get; set; }
        public int id_city { get; set; }
        public Nullable<System.DateTime> date_came { get; set; }
        public Nullable<System.DateTime> birth_date { get; set; }
        public string manager { get; set; }
        public string manager_email { get; set; }
        public string emp_state { get; set; }
        public string position { get; set; }
        public string organization { get; set; }
        public string city { get; set; }
        public string department { get; set; }
        public byte[] photo { get; set; }
        public Nullable<int> is_chief { get; set; }
        public int male { get; set; }
        public int id_position_org { get; set; }
        public string position_org { get; set; }
        public int has_ad_account { get; set; }
        public string ad_login { get; set; }
        public int is_hidden { get; set; }
        public Nullable<int> id_budget { get; set; }
    }
}
