using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GestionProduits.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class MarquesController : GenericCrudController<MarqueDto>
{
    public MarquesController(IDataRepository<MarqueDto> repository)
        : base(repository)
    {
    }
}
