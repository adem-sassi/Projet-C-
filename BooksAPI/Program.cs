using BooksAPI.Data;
using BooksAPI.Models;
using BooksAPI.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Configurer EF Core / SQLite
builder.Services.AddDbContext<BooksContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Injecter le repository générique
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 3) Ajouter les controllers
builder.Services.AddControllers();

// 4) Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BooksAPI v1"));
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Appliquer les migrations et créer la base si nécessaire
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BooksContext>();
    db.Database.Migrate();
}

app.Run();
