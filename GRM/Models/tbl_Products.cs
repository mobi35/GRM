











namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;
    public partial class tbl_Products
    {
        [Key]
        public int productId { get; set; }

        [DisplayName("Product Name")]
        [Required]


        public string productName { get; set; }

        [DisplayName("Product Description")]


        [AllowHtml]
        public string productDescription { get; set; }

        public DateTime? dateRegistered { get; set; }

        public int categoryId { get; set; }
        [DisplayName("Price")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Number only")]

        public double? productPrice { get; set; }
        public string productImage { get; set; }
        public string packageBy { get; set; }
        [NotMapped]
        public string errorMessage { get; set; }
        [NotMapped]
        public string successMessage { get; set; }
        [NotMapped]
        public bool isChecked { get; set; }

        public string isArchive { get; set; }
        public int? bought { get; set; }
    }
}






