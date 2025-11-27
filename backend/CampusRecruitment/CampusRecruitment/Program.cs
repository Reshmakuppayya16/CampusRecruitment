var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//Register Database helper for dependency injection
builder.Services.AddSingleton<CampusRecruitment.Data.DatabaseHelper>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add CORS policy to allow Angular app
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Enable the cors policy (before MapControllers)
app.UseCors("AllowAngularClient");

app.UseAuthorization();

app.MapControllers();

app.Run();
