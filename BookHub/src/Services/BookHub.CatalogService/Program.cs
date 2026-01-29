using BookHub.CatalogService.Application.Services;
using BookHub.CatalogService.Domain.Ports;
using BookHub.CatalogService.Infrastructure.Persistence;
using BookHub.CatalogService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BookHub Catalog Service", Version = "v1" });
});

// Infrastructure
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Domain Ports -> Infrastructure Adapters
builder.Services.AddScoped<IBookRepository, BookRepository>();

// Application Services
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    db.Database.Migrate();
    await DataSeeder.SeedAsync(db);
}

app.Run();
