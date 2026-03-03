using AgoraCommerce.Application.Features.Catalog.Products;
using FluentAssertions;

namespace AgoraCommerce.UnitTests.Catalog;

public class CreateProductValidatorTests
{
    [Fact]
    public void Should_Fail_When_Price_Is_Negative()
    {
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand(
            "SKU-1",
            "Product",
            null,
            -1,
            "GBP",
            Guid.NewGuid(),
            null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Price");
    }
}
