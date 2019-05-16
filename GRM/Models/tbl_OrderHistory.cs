namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_OrderHistory
    {
        [Key]
        public int historyId { get; set; }

        public int clientId { get; set; }
 

        public DateTime? dateRegistered { get; set; }

        public int? isApproved { get; set; }

        [Required]
        [StringLength(50)]
        public string orderId { get; set; }

        public double? totalPrice { get; set; }

        [Column(TypeName = "date")]
        public DateTime? deliveryDate { get; set; }
        [StringLength(50)]
        public string branch { get; set; }

        
    }
}
