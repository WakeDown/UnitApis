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
    
    public partial class recruit_question_form_relatives
    {
        public int id { get; set; }
        public string sid { get; set; }
        public int id_question_form { get; set; }
        public string relation_degree { get; set; }
        public string name { get; set; }
        public Nullable<System.DateTime> birth_date { get; set; }
        public string birth_place { get; set; }
        public string work_place { get; set; }
        public string address { get; set; }
        public Nullable<System.DateTime> dattim1 { get; set; }
        public bool enabled { get; set; }
    }
}
