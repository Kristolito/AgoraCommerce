using AgoraCommerce.Application.Features.Catalog.Categories;
using AgoraCommerce.Application.Features.Catalog.Products;
using AgoraCommerce.Application.Features.Basket;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AgoraCommerce.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBasketService, BasketService>();

        services.AddScoped<IValidator<CreateCategoryCommand>, CreateCategoryCommandValidator>();
        services.AddScoped<IValidator<UpdateCategoryCommand>, UpdateCategoryCommandValidator>();
        services.AddScoped<IValidator<CreateProductCommand>, CreateProductCommandValidator>();
        services.AddScoped<IValidator<UpdateProductCommand>, UpdateProductCommandValidator>();
        services.AddScoped<IValidator<GetOrCreateBasketCommand>, GetOrCreateBasketCommandValidator>();
        services.AddScoped<IValidator<GetBasketQuery>, GetBasketQueryValidator>();
        services.AddScoped<IValidator<AddBasketItemCommand>, AddBasketItemCommandValidator>();
        services.AddScoped<IValidator<UpdateBasketItemQuantityCommand>, UpdateBasketItemQuantityCommandValidator>();
        services.AddScoped<IValidator<RemoveBasketItemCommand>, RemoveBasketItemCommandValidator>();
        services.AddScoped<IValidator<ClearBasketCommand>, ClearBasketCommandValidator>();

        return services;
    }
}
