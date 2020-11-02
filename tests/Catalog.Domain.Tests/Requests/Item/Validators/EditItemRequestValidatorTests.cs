﻿using Catalog.Domain.Entities;
using Catalog.Domain.Requests.Artist;
using Catalog.Domain.Requests.Genre;
using Catalog.Domain.Requests.Item;
using Catalog.Domain.Requests.Item.Validators;
using Catalog.Domain.Services;
using FluentValidation.TestHelper;
using Moq;
using System;
using Xunit;

namespace Catalog.Domain.Tests.Requests.Item.Validators
{
    public class EditItemRequestValidatorTests
    {
        private readonly Mock<IArtistService> _artistServiceMock;
        private readonly Mock<IGenreService> _genreServiceMock;
        private readonly EditItemRequestValidator _validator;

        public EditItemRequestValidatorTests()
        {
            _artistServiceMock = new Mock<IArtistService>();
            _artistServiceMock
                .Setup(x => x.GetArtistAsync(It.IsAny<GetArtistRequest>()))
                .ReturnsAsync(() => null);

            _genreServiceMock = new Mock<IGenreService>();
            _genreServiceMock
                .Setup(x => x.GetGenreAsync(It.IsAny<GetGenreRequest>()))
                .ReturnsAsync(() => null);

            _validator = new EditItemRequestValidator(_artistServiceMock.Object, _genreServiceMock.Object);
        }

        [Fact]
        public void Should_have_error_when_Id_is_null()
        {
            var editItemRequest = new EditItemRequest { Price = new Price() };
            _validator.ShouldHaveValidationErrorFor(x => x.Id, editItemRequest);
        }

        [Fact]
        public void Should_have_error_when_ArtistId_is_null()
        {
            var editItemRequest = new EditItemRequest { Price = new Price() };
            _validator.ShouldHaveValidationErrorFor(x => x.ArtistId, editItemRequest);
        }

        [Fact]
        public void Should_have_error_when_GenreId_is_null()
        {
            var editItemRequest = new EditItemRequest { Price = new Price() };
            _validator.ShouldHaveValidationErrorFor(x => x.GenreId, editItemRequest);
        }

        [Fact]
        public void Should_have_error_when_ArtistId_doesnt_exist()
        {
            _artistServiceMock
                .Setup(x => x.GetArtistAsync(It.IsAny<GetArtistRequest>()))
                .ReturnsAsync(() => null);

            var editItemRequest = new EditItemRequest { Price = new Price(), ArtistId = Guid.NewGuid() };
            _validator.ShouldHaveValidationErrorFor(x => x.ArtistId, editItemRequest);
        }

        [Fact]
        public void Should_have_error_when_GenreId_doesnt_exist()
        {
            _genreServiceMock
                .Setup(x => x.GetGenreAsync(It.IsAny<GetGenreRequest>()))
                .ReturnsAsync(() => null);

            var editItemRequest = new EditItemRequest { Price = new Price(), GenreId = Guid.NewGuid() };
            _validator.ShouldHaveValidationErrorFor(x => x.GenreId, editItemRequest);
        }
    }
}
