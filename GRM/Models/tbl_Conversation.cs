namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Conversation
    {
        [Key]
        public int conversationId { get; set; }

        public int? toUser { get; set; }

        public int? fromUser { get; set; }

        [StringLength(50)]
        public string message { get; set; }

        public DateTime? dateSent { get; set; }

        public int? isRead { get; set; }
    }
}
