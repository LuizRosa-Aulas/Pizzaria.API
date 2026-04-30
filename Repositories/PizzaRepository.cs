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

    public async Task<List<Pizza>> GetAllAsync()
    {
        var pizzas = new List<Pizza>();

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(
            "SELECT Id, Nome, Descricao, Preco FROM Pizzas", connection);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            pizzas.Add(new Pizza
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descricao = reader.GetString(2),
                Preco = reader.GetDecimal(3)
            });
        }

        return pizzas;
    }

    public async Task<Pizza?> GetByIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(
            "SELECT Id, Nome, Descricao, Preco FROM Pizzas WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Pizza
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Descricao = reader.GetString(2),
                Preco = reader.GetDecimal(3)
            };
        }

        return null;
    }

    public async Task<Pizza> CreateAsync(Pizza pizza)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(@"
            INSERT INTO Pizzas (Nome, Descricao, Preco)
            VALUES (@Nome, @Descricao, @Preco);
            SELECT LAST_INSERT_ID();", connection);

        command.Parameters.AddWithValue("@Nome", pizza.Nome);
        command.Parameters.AddWithValue("@Descricao", pizza.Descricao);
        command.Parameters.AddWithValue("@Preco", pizza.Preco);

        pizza.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
        return pizza;
    }

    public async Task<bool> UpdateAsync(Pizza pizza)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(@"
            UPDATE Pizzas
            SET Nome = @Nome, Descricao = @Descricao, Preco = @Preco
            WHERE Id = @Id", connection);

        command.Parameters.AddWithValue("@Id", pizza.Id);
        command.Parameters.AddWithValue("@Nome", pizza.Nome);
        command.Parameters.AddWithValue("@Descricao", pizza.Descricao);
        command.Parameters.AddWithValue("@Preco", pizza.Preco);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(
            "DELETE FROM Pizzas WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
