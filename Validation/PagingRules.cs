using FluentValidation;
using HelpDesk.Data.Dto;

namespace HelpDesk.Validation
{
    public static class PagingRules
    {
        public static void AddPagingRules<T>(this AbstractValidator<T> validator) where T : PagedRequest
        {
            validator.RuleFor(x => x.Page).InclusiveBetween(1, 100_000);
            validator.RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }
    }
}
