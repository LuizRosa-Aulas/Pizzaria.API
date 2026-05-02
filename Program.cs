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

// 🔥 CORS LIBERADO GERAL
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

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// 🔥 AQUI É O PULO DO GATO (antes do MapControllers)
app.UseCors("LiberadoGeral");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();