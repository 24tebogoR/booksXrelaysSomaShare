namespace booksXrelaysSomaShare.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public int OfferId { get; set; }

        // SS3 Improvement
        public decimal OfferPrice { get; set; }

        // ss3 Improvement
        public string? BookTitle { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public bool IsCompleted { get; set; }

        public string PaymentMethod { get; set; } = "Cash on Meetup";

        public string? MeetupLocation { get; set; }

        public string? SellerId { get; set; }

        public ApplicationUser? Seller { get; set; }


      
    }
}