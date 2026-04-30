using MySqlConnector;
using Pizzaria.API.Models;

namespace Pizzaria.API.Repositories;

public class UsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<List<Usuario>> GetAllAsync()
    {
        var usuarios = new List<Usuario>();

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(
            "SELECT Id, Nome, Email, Telefone FROM Usuarios", connection);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            usuarios.Add(new Usuario
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Email = reader.GetString(2),
                Telefone = reader.GetString(3)
            });
        }

        return usuarios;
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(
            "SELECT Id, Nome, Email, Telefone FROM Usuarios WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Usuario
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Email = reader.GetString(2),
                Telefone = reader.GetString(3)
            };
        }

        return null;
    }

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(@"
            INSERT INTO Usuarios (Nome, Email, Telefone)
            VALUES (@Nome, @Email, @Telefone);
            SELECT LAST_INSERT_ID();", connection);

        command.Parameters.AddWithValue("@Nome", usuario.Nome);
        command.Parameters.AddWithValue("@Email", usuario.Email);
        command.Parameters.AddWithValue("@Telefone", usuario.Telefone);

        usuario.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
        return usuario;
    }

    public async Task<bool> UpdateAsync(Usuario usuario)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(@"
            UPDATE Usuarios
            SET Nome = @Nome, Email = @Email, Telefone = @Telefone
            WHERE Id = @Id", connection);

        command.Parameters.AddWithValue("@Id", usuario.Id);
        command.Parameters.AddWithValue("@Nome", usuario.Nome);
        command.Parameters.AddWithValue("@Email", usuario.Email);
        command.Parameters.AddWithValue("@Telefone", usuario.Telefone);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(
            "DELETE FROM Usuarios WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
