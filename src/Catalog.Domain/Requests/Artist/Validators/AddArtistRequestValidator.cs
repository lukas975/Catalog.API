using FluentValidation;

namespace Catalog.Domain.Requests.Artist.Validators
{
    public class AddArtistRequestValidator : AbstractValidator<AddArtistRequest>
    {
        public AddArtistRequestValidator()
        {
            RuleFor(artist => artist.ArtistName).NotEmpty();
        }
    }
}
