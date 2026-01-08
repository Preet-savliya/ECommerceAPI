using ECommerceAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// CORS configuration - allowing your specific Live Server origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500") 
              .AllowAnyMethod()  // Specifically allows DELETE, PUT, POST, GET
              .AllowAnyHeader(); // Allows UserRole, UserId, and Content-Type
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// CRITICAL: UseCors must be before UseAuthorization and MapControllers
app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

// If testing locally without SSL, you can comment this out if needed
app.UseHttpsRedirection(); 

app.UseAuthorization();
app.MapControllers();

app.Run();



