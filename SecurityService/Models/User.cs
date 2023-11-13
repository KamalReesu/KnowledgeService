using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace SecurityService.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("user_id")]
        public int LmsUserId { get; set; }
        [Required(ErrorMessage ="Please provide username")]
        public string LmsUserName { get; set; }
        [RegularExpression("^[A-Za-z0-9]*@[a-z]+.com$")]
        [Required(ErrorMessage = "Please provide EmailId")]
        public string LmsEmailId { get; set; }
        [RegularExpression("^[a-zA-Z0-9*%^#_]+$")]
        [MinLength(8, ErrorMessage = "Please provide a password with min 8 characters")]
        [Required(ErrorMessage = "Please provide your password")]
        public string LmsUserPassword { get; set; }
        public long? LmsMobileNumber { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class ViewProfile
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public long? MobileNumber { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class Login
    {
        public string? UserName { get; set; }
        public string? EmailId { get; set; }
        public string UserPassword { get; set; }
        public bool IsAdmin { get; set; }
    }
}
