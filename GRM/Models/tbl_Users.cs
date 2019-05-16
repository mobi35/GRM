
namespace GRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Users
    {
        [Key]
        public int userId { get; set; }

        [StringLength(15, MinimumLength = 5, ErrorMessage = "Minimum of 5 Characters and Maximum of 15 Characters Allowed")]
        [DisplayName("Username")]
        [Required]
        public string userName { get; set; }
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Minimum of 5 Characters and Maximum of 100 Characters Allowed")]
        [DisplayName("Password")]
        [Required]

        public string PassWord { get; set; }

        [DisplayName("First Name")]
        [Required]

        public string firstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        public string lastName { get; set; }

        [DisplayName("Email Address")]
        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Minimum of 5 Characters and Maximum of 50 Characters Allowed")]

        public string emailAddress { get; set; }

        [DisplayName("Home Address")]
        [Required]
        public string homeAddress { get; set; }

        [DisplayName("Gender")]
        public string gender { get; set; }

        public DateTime? dateRegistered { get; set; }

        [StringLength(50)]
        public string position { get; set; }
        public string branchManager { get; set; }
        public string image { get; set; }

        public int? isActive { get; set; }

        [NotMapped]
        public string outputMessage { get; set; }
        [NotMapped]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Minimum of 5 Characters and Maximum of 15 Characters Allowed")]
        [DisplayName("Confirm Password")]

        public string confirmPassword { get; set; }

        public int? tries { get; set; }

        public int? messageNotif { get; set; }

        public int? orderNotif { get; set; }

        public string isLocked { get; set; }

        public double? credits { get; set; }


        [DisplayName("Contact Number")]
        [StringLength(50)]
        public string contactnumber { get; set; }

        public string saturday_operate { get; set; }

        public string sunday_operate { get; set; }
        public int? contract_duration { get; set; }
        [StringLength(50)]
        public string dti_or_sec { get; set; }
        
       public int? payment_period { get; set; }

        public DateTime? credit_added { get; set; }
  
    }
}







