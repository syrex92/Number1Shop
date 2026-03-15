using CatalogService.Models;
using FluentValidation;

namespace CatalogService.Validators
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.ProductTitle).MinimumLength(4).WithMessage("Название продукта должно быть более 3-х символов");
            RuleFor(x => x.Article).GreaterThanOrEqualTo(0).WithMessage("Артикул больше 0");
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Цена должна быть положительна");
        }
    }
}
