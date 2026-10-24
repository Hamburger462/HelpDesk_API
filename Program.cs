using FluentValidation;

using HelpDesk.Configuration;
using HelpDesk.Services.Error;
using HelpDesk.Validation;

namespace HelpDesk;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddContext(builder.Configuration);

        builder.Services.AddServices();

        builder.Services.AddJwtAuth(builder.Configuration);

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddClientCors(builder.Configuration);

        builder.Services.AddDefRateLim();

        builder.Services.AddSwagDoc();

        builder.Services.AddAuthorization();

        builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
        builder.Services.AddControllers(options => options.Filters.Add<FluentValidationFilter>());

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();

            app.UseSwaggerUI(
                options =>
                options.SwaggerEndpoint("/openapi/v1.json", "v1")
            );
        }

        app.UseCors();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRateLimiter();

        app.MapControllers();

        app.Run();
    }
}
