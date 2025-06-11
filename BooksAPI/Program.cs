using BooksAPI.Data;
using BooksAPI.Models;
using BooksAPI.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// config de la base de donné (sqlite)
builder.Services.AddDbContext<BooksContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// injection du repo generique pour accéder au donnée
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// ajout des controlleur pour les routes de l'api
builder.Services.AddControllers();

// config de swagger pour voir l'api dans le navigateur
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// en mode dev, on montre les details des erreurs + swagger dispo
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

// ici on applique les migration et on cree la bdd si elle existe pas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BooksContext>();
    db.Database.Migrate();
}

app.Run();
