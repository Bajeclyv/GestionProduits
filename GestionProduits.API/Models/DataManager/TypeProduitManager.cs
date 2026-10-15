using AutoMapper;
using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.EntityFramework;

namespace GestionProduits.API.Models.DataManager;

public sealed class TypeProduitManager : GenericDataManager<TypeProduit, TypeProduitDto>
{
    public TypeProduitManager(GestionProduitsContext context, IMapper mapper)
        : base(context, mapper)
    {
    }
}
