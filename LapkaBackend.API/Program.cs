using System.Reflection;
using LapkaBackend.Application;
using LapkaBackend.Application.IServices;
using LapkaBackend.Application.Services;
using LapkaBackend.Domain;
using LapkaBackend.Infrastructure.Database.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddDbContext<IDataContext, DataContext>(options => 
    options.UseSqlServer(ConnectionStringBuilder.Build()));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
 
            ValidIssuer = "issuer",
            ValidAudience = "audience",
            IssuerSigningKey = new SymmetricSecurityKey("YhlyL1kqamyhR1Q4FBHrIjOOyd6rtajB"u8.ToArray())
        };
    });
builder.Services.AddMvc()
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


app.MapControllers();
app.Run();