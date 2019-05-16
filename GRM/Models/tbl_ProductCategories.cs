






namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    public partial class tbl_ProductCategories
    {
        [Key]
        public int categoryId { get; set; }

        [DisplayName("Category Name")]

        [StringLength(50)]
        public string name { get; set; }

        [DisplayName("Category Description")]

        [StringLength(50)]

        public string description { get; set; }
        public string catsBy { get; set; }

        public DateTime dateRegistered { get; set; }
        [NotMapped]
        public string successMessage { get; set; }
        [NotMapped]
        public string errorMessage { get; set; }

        public string isArchive { get; set; }

    }
}











