using GestionProduits.API.Models.EntityFramework;
using GestionProduits.API.Models.Repository;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GestionProduits.API.Models.DTO;

namespace GestionProduits.API.Models.DataManager
{
    public class ProduitManager : GenericDataManager<Produit, ProduitDto>, IProduitRepository
    {
        public ProduitManager(GestionProduitsContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<IEnumerable<ProduitDetailDto>> GetAllWithDetailsAsync()
        {
            return await Context.Set<Produit>()
                .ProjectTo<ProduitDetailDto>(Mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ProduitDetailDto?> GetByIdWithDetailsAsync(int id)
        {
            return await Context.Set<Produit>()
                .Where(p => p.ProduitId == id)
                .ProjectTo<ProduitDetailDto>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProduitDto>> GetByMarqueAsync(int idMarque)
        {
            return await Context.Set<Produit>()
                .Where(p => p.MarqueProduit.MarqueId == idMarque)
                .ProjectTo<ProduitDto>(Mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProduitDto>> SearchByNameAsync(string name)
        {
            return await Context.Set<Produit>()
                .Where(p => p.Nom.Contains(name))
                .ProjectTo<ProduitDto>(Mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
