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
    
    public partial class recruit_question_form_educations
    {
        public int id { get; set; }
        public string sid { get; set; }
        public int id_question_form { get; set; }
        public System.DateTime dattim1 { get; set; }
        public Nullable<int> year_start { get; set; }
        public Nullable<int> year_end { get; set; }
        public string place { get; set; }
        public string study_type { get; set; }
        public string faculty { get; set; }
        public string speciality { get; set; }
        public Nullable<bool> enabled { get; set; }
    }
}