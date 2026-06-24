using Microsoft.Data.Sqlite;
using Pizzaria.API.Models;

namespace Pizzaria.API.Repositories;

public class UsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(IConfiguration configuration)
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

    public async Task<List<Usuario>> GetAllAsync()
    {
        var usuarios = new List<Usuario>();

        using var connection = CreateConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nome, Email, Telefone FROM Usuarios";

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
        using var connection = CreateConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Nome, Email, Telefone FROM Usuarios WHERE Id = @Id";
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
        using var connection = CreateConnection();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Usuarios (Nome, Email, Telefone)
            VALUES (@Nome, @Email, @Telefone);
            SELECT last_insert_rowid();";

        command.Parameters.AddWithValue("@Nome", usuario.Nome);
        command.Parameters.AddWithValue("@Email", usuario.Email);
        command.Parameters.AddWithValue("@Telefone", usuario.Telefone);

        usuario.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
        return usuario;
    }

    public async Task<bool> UpdateAsync(Usuario usuario)
    {
        using var connection = CreateConnection();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Usuarios
            SET Nome = @Nome, Email = @Email, Telefone = @Telefone
            WHERE Id = @Id";

        command.Parameters.AddWithValue("@Id", usuario.Id);
        command.Parameters.AddWithValue("@Nome", usuario.Nome);
        command.Parameters.AddWithValue("@Email", usuario.Email);
        command.Parameters.AddWithValue("@Telefone", usuario.Telefone);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = CreateConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Usuarios WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
