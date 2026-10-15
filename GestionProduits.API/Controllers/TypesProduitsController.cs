using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GestionProduits.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class TypesProduitsController : GenericCrudController<TypeProduitDto>
{
    public TypesProduitsController(IDataRepository<TypeProduitDto> repository)
        : base(repository)
    {
    }
}
