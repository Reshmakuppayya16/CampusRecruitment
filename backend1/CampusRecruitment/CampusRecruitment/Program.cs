var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Database helper for dependency injection
builder.Services.AddSingleton<CampusRecruitment.Data.DatabaseHelper>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy for Angular client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Enable Swagger ALWAYS (Dev + Production)
app.UseSwagger();
app.UseSwaggerUI();

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Apply CORS
app.UseCors("AllowAngularClient");

app.UseAuthorization();

app.MapControllers();

app.Run();
