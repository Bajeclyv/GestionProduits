using AutoMapper;
using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.EntityFramework;

namespace GestionProduits.API.Models.DataManager;

public sealed class MarqueManager : GenericDataManager<Marque, MarqueDto>
{
    public MarqueManager(GestionProduitsContext context, IMapper mapper)
        : base(context, mapper)
    {
    }
}
