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
    
    public partial class recruit_selection_states
    {
        public int id { get; set; }
        public string name { get; set; }
        public string sys_name { get; set; }
        public int order_num { get; set; }
        public bool enabled { get; set; }
        public bool show_next_state_btn { get; set; }
        public bool is_active { get; set; }
        public string btn_name { get; set; }
        public bool is_accept { get; set; }
        public bool is_cancel { get; set; }
        public bool show_second_meeting_button { get; set; }
    }
}
