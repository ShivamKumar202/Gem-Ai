using Gem.COMMON.Enum;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Gem.DAL.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public UserStatus Status { get; set; }
        public string ProfileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
