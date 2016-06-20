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
    
    public partial class Requests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Requests()
        {
            this.RequestHistory = new HashSet<RequestHistory>();
        }
    
        public int id { get; set; }
        public Nullable<int> id_position { get; set; }
        public Nullable<int> id_reason { get; set; }
        public string aim { get; set; }
        public string sid_manager { get; set; }
        public string sid_teacher { get; set; }
        public Nullable<int> id_department { get; set; }
        public Nullable<bool> is_subordinates { get; set; }
        public string subordinates { get; set; }
        public string functions { get; set; }
        public string interactions { get; set; }
        public Nullable<bool> is_instructions { get; set; }
        public string success_rates { get; set; }
        public string plan_to_test { get; set; }
        public string plan_after_test { get; set; }
        public string work_place { get; set; }
        public string work_mode { get; set; }
        public string holiday { get; set; }
        public string hospital { get; set; }
        public string business_trip { get; set; }
        public string overtime_work { get; set; }
        public string compensations { get; set; }
        public Nullable<int> probation { get; set; }
        public string salary_to_test { get; set; }
        public string salary_after_test { get; set; }
        public string bonuses { get; set; }
        public Nullable<bool> sex { get; set; }
        public Nullable<int> age_from { get; set; }
        public Nullable<int> age_to { get; set; }
        public string education { get; set; }
        public string last_work { get; set; }
        public string requirements { get; set; }
        public string knowledge { get; set; }
        public string suggestions { get; set; }
        public string workplace { get; set; }
        public Nullable<bool> is_furniture { get; set; }
        public string furniture { get; set; }
        public Nullable<bool> is_pc { get; set; }
        public Nullable<bool> is_telephone { get; set; }
        public Nullable<bool> is_ethalon { get; set; }
        public Nullable<System.DateTime> appearance { get; set; }
        public Nullable<System.DateTime> create_datetime { get; set; }
        public Nullable<System.DateTime> last_change_datetime { get; set; }
        public string sid_contact_person { get; set; }
        public string sid_responsible_person { get; set; }
        public int id_status { get; set; }
        public string sid_creator { get; set; }
        public bool HaveCoordination { get; set; }
        public bool CoordinationPaused { get; set; }
        public bool Enabled { get; set; }
        public string SidReplacedEmployee { get; set; }
        public Nullable<System.Guid> AgreementDocumentUid { get; set; }
        public Nullable<int> AgreementId { get; set; }
        public int id_status_inner { get; set; }
    
        public virtual departments departments { get; set; }
        public virtual positions positions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequestHistory> RequestHistory { get; set; }
        public virtual RequestReasons RequestReasons { get; set; }
        public virtual RequestStatuses RequestStatuses { get; set; }
    }
}
