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
    
    public partial class recruit_vacancies
    {
        public int id { get; set; }
        public System.DateTime dattim1 { get; set; }
        public System.DateTime dattim2 { get; set; }
        public bool enabled { get; set; }
        public string creator_sid { get; set; }
        public string author_sid { get; set; }
        public int id_position { get; set; }
        public int id_department { get; set; }
        public string chief_sid { get; set; }
        public int id_cause { get; set; }
        public string matcher_sid { get; set; }
        public string personal_manager_sid { get; set; }
        public Nullable<System.DateTime> deadline_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public Nullable<int> id_state { get; set; }
        public Nullable<System.DateTime> state_change_date { get; set; }
        public string state_changer_sid { get; set; }
        public string deleter_sid { get; set; }
        public Nullable<System.DateTime> order_end_date { get; set; }
        public Nullable<System.DateTime> claim_end_date { get; set; }
        public Nullable<int> id_city { get; set; }
        public int id_branch_office { get; set; }
        public string comment { get; set; }
        public int id_vacancy_type { get; set; }
    }
}
