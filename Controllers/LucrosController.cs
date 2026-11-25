using Microsoft.AspNetCore.Mvc;
using Pizzaria.API.Models;
using Pizzaria.API.Repositories;

namespace Pizzaria.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LucrosController : ControllerBase
{
    private readonly LucrosRepository _repository;

    public LucrosController(LucrosRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Lucros>>> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

}


