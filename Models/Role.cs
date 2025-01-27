using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Role
    {
        [Key]
        [Column("Id")]
        public string Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("NormalizedName")]
        public string NormalizedName { get; set; }

        [Column("ConcurrencyStamp")]
        public string ConcurrencyStamp { get; set; }
    }
}