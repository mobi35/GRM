namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Rating
    {
        public int id { get; set; }

        [StringLength(50)]
        public string client { get; set; }

        public double? services { get; set; }

        public double? quality { get; set; }

        public double? price_improvement { get; set; }

        public string message { get; set; }

        public DateTime? date_rated { get; set; }
    }
}
