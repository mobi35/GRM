namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Message
    {
        public int id { get; set; }

        public string message { get; set; }

        public DateTime? dateSent { get; set; }

        [StringLength(50)]
        public string sent { get; set; }

        [StringLength(50)]
        public string receive { get; set; }

        [StringLength(50)]
        public string isRead { get; set; }
    }
}
