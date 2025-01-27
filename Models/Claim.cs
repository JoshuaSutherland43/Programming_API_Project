using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Claim
    {
        [Key]
        [Column("CLAIM_ID")] 
        public int ClaimID { get; set; }  

        [Column("UPLOAD_DATE")]
        public DateTime UploadDate { get; set; }

        [Column("HOURS")]
        public int Hours { get; set; }

        [Column("RATE")]
        public double Rate { get; set; }

        [Column("NOTES")]
        public string Notes { get; set; }

        [Column("User_ID")]
        public int UserID { get; set; }

        [Column("FILE_PATH")]
        public string FilePath { get; set; }

        [Column("STATUS")]
        public string Status { get; set; } = "Pending";

        [Column("APPROVED_DATE")]
        public DateTime? ApprovedDate { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }
    }
}
