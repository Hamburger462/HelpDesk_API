
using HelpDesk.Configuration;
using HelpDesk.Services.Error;

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

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
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
