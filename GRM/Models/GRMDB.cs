namespace GRM.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GRMDB : DbContext
    {
        public GRMDB()
            : base("name=GRMDB")
        {
        }

        public virtual DbSet<PageContent> PageContents { get; set; }
        public virtual DbSet<tbl_Clients> tbl_Clients { get; set; }
        public virtual DbSet<tbl_Conversation> tbl_Conversation { get; set; }
        public virtual DbSet<tbl_Feedbacks> tbl_Feedbacks { get; set; }
        public virtual DbSet<tbl_Homepage> tbl_Homepage { get; set; }
        public virtual DbSet<tbl_Message> tbl_Message { get; set; }
        public virtual DbSet<tbl_Notifications> tbl_Notifications { get; set; }
        public virtual DbSet<tbl_OrderHistory> tbl_OrderHistory { get; set; }
        public virtual DbSet<tbl_OrderList> tbl_OrderList { get; set; }
        public virtual DbSet<tbl_PackageItems> tbl_PackageItems { get; set; }
        public virtual DbSet<tbl_Packages> tbl_Packages { get; set; }
        public virtual DbSet<tbl_ProductCategories> tbl_ProductCategories { get; set; }
        public virtual DbSet<tbl_Products> tbl_Products { get; set; }
        public virtual DbSet<tbl_Users> tbl_Users { get; set; }
        public virtual DbSet<tbl_News> tbl_News { get; set; }
        public virtual DbSet<tbl_Rating> tbl_Rating { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
