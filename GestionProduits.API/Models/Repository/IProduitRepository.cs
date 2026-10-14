using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace GestionProduits.API.Models.Repository
{
    public interface IProduitRepository : IDataRepository<ProduitDto>
    {
        Task<IEnumerable<ProduitDetailDto>> GetAllWithDetailsAsync();
        Task<ProduitDetailDto?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<ProduitDto>> GetByMarqueAsync(int idMarque);
        Task<IEnumerable<ProduitDto>> SearchByNameAsync(string name);
    }
}
