using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Ajouter Ocelot JSON
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy =>
        {
            policy.WithOrigins("http://localhost:8080")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Ajouter Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowBlazorClient");

// Ajouter Ocelot au pipeline
await app.UseOcelot();

app.Run();
