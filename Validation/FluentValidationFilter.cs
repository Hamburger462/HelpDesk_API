using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HelpDesk.Validation
{
    // Validates every action argument that has a registered IValidator<T>.
    // Failures are returned as a 400 ValidationProblemDetails, the same shape [ApiController]
    // produces for model-binding errors, so controllers and handlers need no validation code.
    public class FluentValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var services = context.HttpContext.RequestServices;

            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                if (services.GetService(validatorType) is not IValidator validator) continue;

                var result = await validator.ValidateAsync(
                    new ValidationContext<object>(argument), context.HttpContext.RequestAborted);

                foreach (var error in result.Errors)
                    context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            if (!context.ModelState.IsValid)
            {
                var factory = services.GetRequiredService<ProblemDetailsFactory>();
                var problem = factory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);
                context.Result = new BadRequestObjectResult(problem);
                return;
            }

            await next();
        }
    }
}
