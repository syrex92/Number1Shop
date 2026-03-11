using CatalogService.Models;
using FluentValidation;

namespace CatalogService.Validators
{
    public class AddProductValidator: AbstractValidator<CreateProductDto>
    {
        public AddProductValidator()
        {
            RuleFor(x => x.ProductTitle).NotNull().NotEmpty().MinimumLength(4).WithMessage("Название продукта должно быть более 3-х символов");
            RuleFor(x => x.ProductCategory).NotNull().NotEmpty().WithMessage("Указание категории обязательно");
            RuleFor(x => x.Article).NotNull().GreaterThanOrEqualTo(0).WithMessage("Артикул обязателен");
            RuleFor(x => x.Price).NotNull().GreaterThanOrEqualTo(0).WithMessage("Цена должна быть положительна");
        }
    }
}
