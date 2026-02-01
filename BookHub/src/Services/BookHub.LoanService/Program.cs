using BookHub.LoanService.Application.Services;
using BookHub.LoanService.Domain.Ports;
using BookHub.LoanService.Infrastructure.HttpClients;
using BookHub.LoanService.Infrastructure.Persistence;
using BookHub.LoanService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BookHub Loan Service", Version = "v1" });
});

// DbContext
builder.Services.AddDbContext<LoanDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories & Services
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ILoanService, LoanService>();

// Générateur de JWT interne pour les appels inter-services
builder.Services.AddSingleton<BookHub.LoanService.Infrastructure.Security.InternalJwtTokenGenerator>();

// HttpClients pour microservices
builder.Services.AddHttpClient<ICatalogServiceClient, CatalogServiceClient>(client =>
{
    // Utilisation du nom de service Docker pour le réseau interne
    client.BaseAddress = new Uri("http://catalog-service:8080");
});


builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://user-service:8080");
})
// Injection du InternalJwtTokenGenerator dans le constructeur du client
.AddHttpMessageHandler(sp =>
{
    var tokenGenerator = sp.GetRequiredService<BookHub.LoanService.Infrastructure.Security.InternalJwtTokenGenerator>();
    return new InternalJwtHandler(tokenGenerator);
});

// Healthchecks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);

// CORS
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

// Migration automatique
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LoanDbContext>();
    db.Database.Migrate();
}

app.Run();
