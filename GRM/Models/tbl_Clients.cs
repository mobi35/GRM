namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Clients
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int clientId { get; set; }

        [StringLength(50)]
        public string companyName { get; set; }

        [StringLength(50)]
        public string description { get; set; }

        [StringLength(50)]
        public string location { get; set; }

        [StringLength(50)]
        public string contact { get; set; }

        public string logo { get; set; }

        [StringLength(50)]
        public string contractDuration { get; set; }

        public DateTime? dateRegistered { get; set; }

        public int? isActive { get; set; }
    }
}
