using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SecurityService.Models
{
    public partial class User
    {
        [Key]
        public int LmsUserId { get; set; }
        [Required(ErrorMessage ="Please provide username")]
        public string LmsUserName { get; set; }
        [Required(ErrorMessage = "Please provide EmailId")]
        public string LmsEmailId { get; set; }
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
