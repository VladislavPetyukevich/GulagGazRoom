using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    if (builder.Environment.IsDevelopment())
    {
        optionsBuilder.UseSqlite(builder.Configuration.GetConnectionString("sqlite"));
    }
});

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var appDbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    appDbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();