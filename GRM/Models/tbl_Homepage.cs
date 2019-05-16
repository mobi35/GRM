namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Homepage
    {
        [Key]
        public int homepageId { get; set; }

        [StringLength(50)]
        public string whatTag { get; set; }

        [StringLength(50)]
        public string title { get; set; }

        [StringLength(50)]
        public string description { get; set; }

        public string image { get; set; }
    }
}
