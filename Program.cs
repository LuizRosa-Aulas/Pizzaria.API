using Pizzaria.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Repositories
builder.Services.AddScoped<PizzaRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<VendaRepository>();
builder.Services.AddScoped<LucrosRepository>();
builder.Services.AddScoped<AcompanhamentoRepository>();

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

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("LiberadoGeral");

app.MapControllers();

app.Run();
