using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebService.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure MySQL database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 30))));

// Add Swagger generation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = string.Empty;  // Optionally, set to empty to serve Swagger UI at root
    });
}

app.UseHttpsRedirection();

// Serve static files (images) from wwwroot folder
app.UseStaticFiles();

app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run the application
app.Run();
