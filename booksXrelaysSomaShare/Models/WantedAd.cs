using System.ComponentModel.DataAnnotations;

namespace booksXrelaysSomaShare.Models
{
    public class WantedAd
    {

        public int WantedAdId { get; set; }

        [Required(ErrorMessage = "Please Enter book title")]
        public string BookTitle { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }
    }
}
