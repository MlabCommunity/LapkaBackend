using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using LapkaBackend.Application;
using LapkaBackend.Application.Helper;
using LapkaBackend.Application.Intercepters;
using LapkaBackend.Application.Mappers;
using LapkaBackend.Domain.Records;
using LapkaBackend.Infrastructure;
using LapkaBackend.Infrastructure.Data;
using LapkaBackend.Infrastructure.Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Extensions = LapkaBackend.Infrastructure.Extensions;

namespace LapkaBackend.API;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e =>
                        {
                            try
                            {
                                return JsonSerializer.Deserialize<Error>(e.ErrorMessage);
                            }
                            catch (Exception)
                            {
                                return new Error("invalid_request", e.ErrorMessage);
                            }
                        });
                    var errorsWrapper = new
                    {
                        errors
                    };
                    return new BadRequestObjectResult(JsonSerializer.SerializeToElement(errorsWrapper));
                };
            });
        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddValidatorsFromAssembly(Assembly.Load("LapkaBackend.Application"));
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddTransient<IValidatorInterceptor, CustomIntercepter>();
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddAutoMapper(typeof(UserMappingProfile));
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        builder.Services.AddHttpContextAccessor();
        
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using Bearer (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
            options.SupportNonNullableReferenceTypes();
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        builder.Services.AddControllers()
            .ConfigureApiBehaviorOptions(opt =>
            {
                opt.SuppressMapClientErrors = true;
            });
        
        builder.Services.AddHealthChecks();

        var app = builder.Build();
    
        using (var scope = app.Services.CreateScope())
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(builder.Configuration.GetConnectionString("MySql"))
                .Options;
            
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            db.Database.Migrate();
            Extensions.Seed(options);
            var job = scope.ServiceProvider.GetRequiredService<UpdateDeleteJob>();
            RecurringJob.AddOrUpdate("deleteJob",() => job.PermDelete(), Cron.Daily);
            RecurringJob.TriggerJob("deleteJob");
        }

        app.MapHealthChecks("/healthcheck");

        app.UseHangfireDashboard("/hangfire");
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();
        
        //app.UseMiddleware<ErrorHandlerMiddleware>();

        app.MapControllers();

        app.Run();
    }
}