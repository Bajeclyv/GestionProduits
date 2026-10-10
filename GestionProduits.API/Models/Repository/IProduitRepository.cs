using GestionProduits.API.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace GestionProduits.API.Models.Repository
{
    public interface IProduitRepository : IDataRepository<Produit>
    {
        Task<IEnumerable<Produit>> GetAllWithDetailsAsync();
        Task<Produit?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Produit>> GetByMarqueAsync(int idMarque);
        Task<IEnumerable<Produit>> SearchByNameAsync(string name);
    }
}
