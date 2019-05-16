using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GRM.Models
{
    public class ProductItem
    {

        public tbl_Products Product
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

    }
}