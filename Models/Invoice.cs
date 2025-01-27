using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public partial class Invoice
    {
        [Key]
        [Column("INVOICE_ID")]  
        public int InvoiceID { get; set; }

        [Column("CLAIM_ID")]
        public int ClaimID { get; set; }

        [Column("User_ID")]
        public int UserID { get; set; }

        [Column("TOTAL_AMOUNT")]
        public double TotalAmount { get; set; }

        [Column("INVOICE_DATE")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        // Foreign Key Relationships
        [ForeignKey("CLAIM_ID")]
        public Claim Claim { get; set; }

        [ForeignKey("User_ID")]
        public User User { get; set; }
    }
}