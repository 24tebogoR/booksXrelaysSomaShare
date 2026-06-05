using System.ComponentModel.DataAnnotations;

namespace booksXrelaysSomaShare.Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        public int TransactionId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please enter a review comment")]
        public string Comment { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.Now;
    }
}
