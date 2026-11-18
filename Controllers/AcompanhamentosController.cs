using Microsoft.AspNetCore.Mvc;
using Pizzaria.API.Models;
using Pizzaria.API.Repositories;

namespace Pizzaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AcompanhamentosController : ControllerBase
{
    private readonly AcompanhamentoRepository _repository;

    public AcompanhamentosController(AcompanhamentoRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Acompanhamento>>> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Acompanhamento>> GetById(int id)
    {
        var acompanhamento = await _repository.GetByIdAsync(id);
        if (acompanhamento == null) return NotFound();
        return Ok(acompanhamento);
    }

    [HttpPost]
    public async Task<ActionResult<Acompanhamento>> Create(Acompanhamento acompanhamento)
    {
        var created = await _repository.CreateAsync(acompanhamento);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Acompanhamento acompanhamento)
    {
        acompanhamento.Id = id;
        if (!await _repository.UpdateAsync(acompanhamento)) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (!await _repository.DeleteAsync(id)) return NotFound();
        return NoContent();
    }
}
