using MySqlConnector;
using Pizzaria.API.Models;

namespace Pizzaria.API.Repositories;

public class VendaRepository
{
    private readonly string _connectionString;

    public VendaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private async Task<MySqlConnection> CreateConnectionAsync()
    {
        var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }

    public async Task<List<Venda>> GetAllAsync()
    {
        var vendas = new List<Venda>();

        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT v.Id, v.UsuarioId, v.PizzaId, v.Quantidade, v.ValorTotal, v.DataVenda,
                   u.Nome AS NomeUsuario, p.Nome AS NomePizza
            FROM Vendas v
            INNER JOIN Usuarios u ON u.Id = v.UsuarioId
            INNER JOIN Pizzas p ON p.Id = v.PizzaId
            ORDER BY v.DataVenda DESC";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            vendas.Add(new Venda
            {
                Id = reader.GetInt32(0),
                UsuarioId = reader.GetInt32(1),
                PizzaId = reader.GetInt32(2),
                Quantidade = reader.GetInt32(3),
                ValorTotal = reader.GetDecimal(4),
                DataVenda = reader.GetDateTime(5),
                NomeUsuario = reader.GetString(6),
                NomePizza = reader.GetString(7)
            });
        }

        return vendas;
    }

    public async Task<Venda?> GetByIdAsync(int id)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT v.Id, v.UsuarioId, v.PizzaId, v.Quantidade, v.ValorTotal, v.DataVenda,
                   u.Nome AS NomeUsuario, p.Nome AS NomePizza
            FROM Vendas v
            INNER JOIN Usuarios u ON u.Id = v.UsuarioId
            INNER JOIN Pizzas p ON p.Id = v.PizzaId
            WHERE v.Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Venda
            {
                Id = reader.GetInt32(0),
                UsuarioId = reader.GetInt32(1),
                PizzaId = reader.GetInt32(2),
                Quantidade = reader.GetInt32(3),
                ValorTotal = reader.GetDecimal(4),
                DataVenda = reader.GetDateTime(5),
                NomeUsuario = reader.GetString(6),
                NomePizza = reader.GetString(7)
            };
        }

        return null;
    }

    public async Task<Venda> CreateAsync(Venda venda)
    {
        using var connection = await CreateConnectionAsync();

        var dataVenda = DateTime.Now;

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Vendas (UsuarioId, PizzaId, Quantidade, ValorTotal, DataVenda)
            VALUES (@UsuarioId, @PizzaId, @Quantidade, @ValorTotal, @DataVenda)";

        command.Parameters.AddWithValue("@UsuarioId", venda.UsuarioId);
        command.Parameters.AddWithValue("@PizzaId", venda.PizzaId);
        command.Parameters.AddWithValue("@Quantidade", venda.Quantidade);
        command.Parameters.AddWithValue("@ValorTotal", venda.ValorTotal);
        command.Parameters.AddWithValue("@DataVenda", dataVenda);

        await command.ExecuteNonQueryAsync();

        venda.Id = (int)command.LastInsertedId;
        venda.DataVenda = dataVenda;
        return venda;
    }

    public async Task<bool> UpdateAsync(Venda venda)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Vendas
            SET UsuarioId = @UsuarioId, PizzaId = @PizzaId, Quantidade = @Quantidade, ValorTotal = @ValorTotal
            WHERE Id = @Id";

        command.Parameters.AddWithValue("@Id", venda.Id);
        command.Parameters.AddWithValue("@UsuarioId", venda.UsuarioId);
        command.Parameters.AddWithValue("@PizzaId", venda.PizzaId);
        command.Parameters.AddWithValue("@Quantidade", venda.Quantidade);
        command.Parameters.AddWithValue("@ValorTotal", venda.ValorTotal);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = await CreateConnectionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Vendas WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
