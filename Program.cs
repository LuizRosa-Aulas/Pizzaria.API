using MySqlConnector;
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

// Inicializar banco MySQL na primeira execução
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
await InicializarBancoAsync(connectionString);

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("LiberadoGeral");

app.MapControllers();

app.Run();

// Cria as tabelas e os dados de exemplo. O container do MySQL leva alguns segundos
// para aceitar conexões, então tentamos algumas vezes antes de desistir.
static async Task InicializarBancoAsync(string connectionString)
{
    var scriptPath = new[]
    {
        Path.Combine(AppContext.BaseDirectory, "init.sql"),
        Path.Combine(AppContext.BaseDirectory, "Scripts", "CriarBanco.sql")
    }.FirstOrDefault(File.Exists)
        ?? throw new FileNotFoundException("Script de criação do banco não encontrado (init.sql ou Scripts/CriarBanco.sql).");

    var script = await File.ReadAllTextAsync(scriptPath);

    const int maxTentativas = 15;
    for (var tentativa = 1; ; tentativa++)
    {
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = script;
            await command.ExecuteNonQueryAsync();

            Console.WriteLine("Banco inicializado com sucesso.");
            return;
        }
        catch (MySqlException ex) when (tentativa < maxTentativas)
        {
            Console.WriteLine($"MySQL indisponível (tentativa {tentativa}/{maxTentativas}): {ex.Message}");
            await Task.Delay(TimeSpan.FromSeconds(4));
        }
    }
}
