using Microsoft.Data.Sqlite;
using Pizzaria.API.Models;

namespace Pizzaria.API.Repositories;

public class PizzaRepository
{
    private readonly string _connectionString;

    public PizzaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqliteConnection CreateConnection()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "PRAGMA foreign_keys=ON;";
        cmd.ExecuteNonQuery();
        return conn;
    }

    public async Task<List<Pizza>> GetAllAsync()
    {
        var pizzas = new List<Pizza>();

        using var connection = CreateConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nome, Descricao, Preco FROM Pizzas";

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
        using var connection = CreateConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nome, Descricao, Preco FROM Pizzas WHERE Id = @Id";
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
        using var connection = CreateConnection();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Pizzas (Nome, Descricao, Preco)
            VALUES (@Nome, @Descricao, @Preco);
            SELECT last_insert_rowid();";

        command.Parameters.AddWithValue("@Nome", pizza.Nome);
        command.Parameters.AddWithValue("@Descricao", pizza.Descricao);
        command.Parameters.AddWithValue("@Preco", pizza.Preco);

        pizza.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
        return pizza;
    }

    public async Task<bool> UpdateAsync(Pizza pizza)
    {
        using var connection = CreateConnection();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Pizzas
            SET Nome = @Nome, Descricao = @Descricao, Preco = @Preco
            WHERE Id = @Id";

        command.Parameters.AddWithValue("@Id", pizza.Id);
        command.Parameters.AddWithValue("@Nome", pizza.Nome);
        command.Parameters.AddWithValue("@Descricao", pizza.Descricao);
        command.Parameters.AddWithValue("@Preco", pizza.Preco);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = CreateConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Pizzas WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
