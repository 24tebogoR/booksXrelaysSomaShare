using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace booksXrelaysSomaShare.Validators
{
    public class UniversityEmailAttribute : ValidationAttribute
    {
        private readonly string[] allowedDomains =
        {
            "@stadio.ac.za",
            "@up.ac.za",
            "@tuks.co.za",
            "@uct.ac.za",
            "@wits.ac.za"
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

            if (string.IsNullOrEmpty(email))
                return new ValidationResult("University email is required.");

            bool valid = allowedDomains.Any(d => email.ToLower().EndsWith(d));

            if (!valid)
                return new ValidationResult("Only university email addresses are allowed.");

            return ValidationResult.Success;
        }
    }
}