namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Notifications
    {
        public int? messageNotif { get; set; }

        public int? feedbackNotif { get; set; }

        public int? clientOrderNotif { get; set; }

        [Key]
        [Column("int")]
        public int _int { get; set; }
    }
}
