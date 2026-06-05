using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace booksXrelaysSomaShare.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string StudentNumber { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [Range(17, 100)]
        public int Age { get; set; } 
        [Required]
        public double TrustScore { get; set; } = 5.0;
        [Required]
        public int SuccessfulTransactions { get; set; } = 0;

        [Required]
        public string CampusName { get; set; } = string.Empty;

        [Required]

        // SS3 Improvement: Indicates whether the user has earned trusted seller status
        public bool IsTrustedSeller { get; set; } = false;

    }
}