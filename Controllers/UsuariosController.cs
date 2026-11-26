using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Pizzaria.API.Models;
using Pizzaria.API.Repositories;

namespace Pizzaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioRepository _repository;

    public UsuariosController(UsuarioRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Usuario>>> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetById(int id)
    {
        var usuario = await _repository.GetByIdAsync(id);
        if (usuario == null) return NotFound();
        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> Create(Usuario usuario)
    {
        var created = await _repository.CreateAsync(usuario);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Usuario usuario)
    {
        usuario.Id = id;
        if (!await _repository.UpdateAsync(usuario)) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            if (!await _repository.DeleteAsync(id)) return NotFound();
            return NoContent();
        }
        catch (MySqlException ex) when (ex.ErrorCode is MySqlErrorCode.RowIsReferenced2
                                          or MySqlErrorCode.RowIsReferenced)
        {
            // O banco barra a exclusão por chave estrangeira: responde 409 com o motivo.
            // 1451 ou 1217: o MySQL usa o 1217 quando a conta não pode ver a tabela filha.
            return Conflict(new { mensagem = "Este usuário tem vendas registradas e não pode ser excluído." });
        }
    }
}
