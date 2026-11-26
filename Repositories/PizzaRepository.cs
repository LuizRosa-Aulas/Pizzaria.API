using MySqlConnector;
using Pizzaria.API.Models;

namespace Pizzaria.API.Repositories;

public class PizzaRepository
{
    private readonly string _connectionString;

    public PizzaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private async Task<MySqlConnection> CreateConnectionAsync()
    {
        var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }

    public async Task<List<Pizza>> GetAllAsync()
    {
        var pizzas = new List<Pizza>();

        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nome, Descricao, Preco, Custo, Obs, Qtd FROM Pizzas";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            pizzas.Add(new Pizza
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descricao = reader.GetString(2),
                Preco = reader.GetDecimal(3),
                Custo = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                Obs = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Qtd = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
            });
        }

        return pizzas;
    }

    public async Task<Pizza?> GetByIdAsync(int id)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nome, Descricao, Preco, Custo, Obs, Qtd FROM Pizzas WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Pizza
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descricao = reader.GetString(2),
                Preco = reader.GetDecimal(3),
                Custo = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                Obs = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Qtd = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
            };
        }

        return null;
    }

    public async Task<Pizza> CreateAsync(Pizza pizza)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Pizzas (Nome, Descricao, Preco, Custo, Obs, Qtd)
            VALUES (@Nome, @Descricao, @Preco, @Custo, @Obs, @Qtd)";

        command.Parameters.AddWithValue("@Nome", pizza.Nome);
        command.Parameters.AddWithValue("@Descricao", pizza.Descricao);
        command.Parameters.AddWithValue("@Preco", pizza.Preco);
        command.Parameters.AddWithValue("@Custo", pizza.Custo);
        command.Parameters.AddWithValue("@Obs", pizza.Obs);
        command.Parameters.AddWithValue("@Qtd", pizza.Qtd);

        await command.ExecuteNonQueryAsync();

        pizza.Id = (int)command.LastInsertedId;
        return pizza;
    }

    public async Task<bool> UpdateAsync(Pizza pizza)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Pizzas
            SET Nome = @Nome, Descricao = @Descricao, Preco = @Preco, Custo = @Custo, Obs = @Obs, Qtd = @Qtd
            WHERE Id = @Id";

        command.Parameters.AddWithValue("@Id", pizza.Id);
        command.Parameters.AddWithValue("@Nome", pizza.Nome);
        command.Parameters.AddWithValue("@Descricao", pizza.Descricao);
        command.Parameters.AddWithValue("@Preco", pizza.Preco);
        command.Parameters.AddWithValue("@Custo", pizza.Custo);
        command.Parameters.AddWithValue("@Obs", pizza.Obs);
        command.Parameters.AddWithValue("@Qtd", pizza.Qtd);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Pizzas WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<List<Tipo>> GetAllTipoAsync()
    {
        var tipos = new List<Tipo>();

        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nome FROM Tipo_pizza";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tipos.Add(new Tipo
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1)
            });
        }

        return tipos;
    }

    public async Task<List<Borda>> GetAllBordaAsync()
    {
        var bordas = new List<Borda>();

        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, Nome, Valor FROM Borda_pizza";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            bordas.Add(new Borda
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Valor = reader.GetFloat(2)
            });
        }

        return bordas;
    }
}