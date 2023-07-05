using LapkaBackend.Domain.Common;
using LapkaBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddControllers();
//builder.Services.AddApplication();
//builder.Services.AddInfrastructure();
//builder.Services.AddDbContext<LapkaBackendDBContext>();

// Dodaj konfiguracjê DbContext i interfejsu do wstrzykiwania zale¿noœci.
builder.Services.AddDbContext<ILapkaBackendDbContext, LapkaBackendDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Server=(localdb)\\MSSQLLocalDB; Database=LapkaBackend;Trusted_Connection=True;")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();