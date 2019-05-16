namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Packages
    {
        [Key]
        public int packageId { get; set; }

        [StringLength(50)]
        public string packageName { get; set; }

        public DateTime? dateRegistered { get; set; }

        [StringLength(50)]
        public string accountType { get; set; }

        public int? clientId { get; set; }

        public double? packagePrice { get; set; }
    }
}
