using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using LapkaBackend.Application;
using LapkaBackend.Infrastructure;
using System.Reflection;
//using LapkaBackend.Application.Exceptions;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddApplication();
        builder.Services.AddInfrasturcture(builder.Configuration);


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
        //TODO: Spr�bowa� wy�aczy� cia�o na 400 error

        //builder.Services.AddScoped<IUserService, UserService>();


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
        var app = builder.Build();

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