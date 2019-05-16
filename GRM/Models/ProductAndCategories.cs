using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRM.Models
{
    public partial class ProductAndCategories
    {
        public tbl_ProductCategories category_model { get; set; }
        public tbl_Products product_model { get; set; }
    }
}