namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PageContent")]
    public partial class PageContent
    {
        public int Id { get; set; }

        public int? location { get; set; }

        public string contents { get; set; }

        public DateTime? datepub { get; set; }
    }
}
