
namespace booksXrelaysSomaShare.Models
{
    public class Offer
    {
        public int OfferId { get; set; }

        public decimal OfferPrice { get; set; }

        public int TextbookId{ get; set; }

        public Textbook? Textbook { get; set; }


        public string? UserId { get; set; }

        public string? UserEmail { get; set; }

        public bool IsAccepted { get; set; }
    }
}