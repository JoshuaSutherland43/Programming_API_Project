using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace API.Models
{
    public partial class User
    {
        [Key]
        [Column("User_ID")]
        public int UserID { get; set; }

        [Column("FIRST_NAME")]
        public string? FirstName { get; set; }

        [Column("LAST_NAME")]
        public string? LastName { get; set; }

        [Column("EMAIL")]
        public string? Email { get; set; }

        [Column("PASSWORD")]
        public string? Password { get; set; }

        [Column("ROLE_ID")]
        public string? RoleId { get; set; }

        // Navigation Property for Claims
        public virtual ICollection<Claim>? Claims { get; set; }

        [ForeignKey("ROLE_ID")]
        public Role Role { get; set; } 
    }
}