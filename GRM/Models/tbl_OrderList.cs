namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_OrderList
    {
        [StringLength(50)]
        public string orderId { get; set; }

        public int? clientId { get; set; }

        public int? packageId { get; set; }

        public int? quantity { get; set; }

        public int? usageDate { get; set; }

        public int id { get; set; }

        public double? price { get; set; }

        [StringLength(50)]
        public string prodName { get; set; }
        [StringLength(50)]
        public string branch { get; set; }
        
    }
}
