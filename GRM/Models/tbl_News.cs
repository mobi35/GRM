






namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_News
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Required]
        public string title { get; set; }
        [Required]

        public string content { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? datapublish { get; set; }

        [StringLength(50)]
        public string employee { get; set; }

        public string featureImage { get; set; }
        public string isArchive { get; set; }
    }
}








