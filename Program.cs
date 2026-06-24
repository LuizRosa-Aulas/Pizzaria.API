using Microsoft.Data.Sqlite;
using Pizzaria.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Repositories
builder.Services.AddScoped<PizzaRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<VendaRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LiberadoGeral", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Inicializar banco SQLite na primeira execução
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
using (var initConn = new SqliteConnection(connectionString))
{
    initConn.Open();
    using var pragma1 = initConn.CreateCommand();
    pragma1.CommandText = "PRAGMA journal_mode=WAL;";
    pragma1.ExecuteNonQuery();

    using var pragma2 = initConn.CreateCommand();
    pragma2.CommandText = "PRAGMA foreign_keys=ON;";
    pragma2.ExecuteNonQuery();

    var initSql = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "init.sql"));
    using var cmd = initConn.CreateCommand();
    cmd.CommandText = initSql;
    cmd.ExecuteNonQuery();
}

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("LiberadoGeral");

app.MapControllers();

app.Run();
