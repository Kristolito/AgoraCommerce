using AgoraCommerce.Application.Features.Catalog.Categories;
using FluentAssertions;

namespace AgoraCommerce.UnitTests.Catalog;

public class CreateCategoryValidatorTests
{
    [Fact]
    public void Should_Fail_When_Name_Is_Empty()
    {
        var validator = new CreateCategoryCommandValidator();
        var command = new CreateCategoryCommand(string.Empty, null);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Name");
    }
}
