//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiceSheetManager.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EquipmentLocation
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public string ScannedUserName { get; set; }
        public string ScannedUserFirstName { get; set; }
        public string ScannedUserSurname { get; set; }
        public int CanvasSubmissionNumber { get; set; }
        public int LocationCode { get; set; }
        public System.DateTime DtScanned { get; set; }
        public string Notes { get; set; }
    
        public virtual Equipment Equipment { get; set; }
    }
}
