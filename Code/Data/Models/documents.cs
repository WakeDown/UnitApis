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
    
    public partial class documents
    {
        public int id { get; set; }
        public System.Guid data_sid { get; set; }
        public byte[] summary { get; set; }
        public byte[] data { get; set; }
        public string name { get; set; }
        public System.DateTime dattim1 { get; set; }
        public System.DateTime dattim2 { get; set; }
        public bool enabled { get; set; }
        public string creator_sid { get; set; }
        public string deleter_sid { get; set; }
        public Nullable<int> folder_id { get; set; }
    
        public virtual document_folders document_folders { get; set; }
    }
}