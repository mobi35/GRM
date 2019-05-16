namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_PackageItems
    {
        public int packageItemId { get; set; }

        [StringLength(50)]
        public string item { get; set; }

        public int packageId { get; set; }

        public int id { get; set; }

        public int? quantity { get; set; }

        public double? price { get; set; }
    }
}
