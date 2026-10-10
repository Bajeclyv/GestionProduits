using GestionProduits.API.Models.EntityFramework;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GestionProduits.API.Models.DataManager
{
    public class ProduitManager : IDataRepository<Produit>
    {
        readonly GestionProduitsContext _context;

        public ProduitManager(GestionProduitsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Produit>> GetAllAsync()
        {
            return await _context.Produits.ToListAsync();
        }

        public async Task<Produit?> GetByIdAsync(int id)
        {
            return await _context.Produits.FirstOrDefaultAsync(u => u.ProduitId == id);
        }

        public async Task AddAsync(Produit entity)
        {
            await _context.Produits.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Produit produit, Produit entity)
        {
            _context.Entry(produit).State = EntityState.Modified;
            produit.ProduitId = entity.ProduitId;
            produit.NomProduit = entity.NomProduit;
            produit.Description = entity.Description;
            produit.NomPhoto = entity.NomPhoto;
            produit.UriPhoto = entity.UriPhoto;
            produit.MarqueProduit = entity.MarqueProduit;
            produit.Type = entity.Type;
            produit.StockReel = entity.StockReel;
            produit.StockMin = entity.StockMin;
            produit.StockMax = entity.StockMax;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Produit produit)
        {
            _context.Produits.Remove(produit);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Produit>> GetAllWithDetailsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Produit?> GetByIdWithDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Produit>> GetByMarqueAsync(int idMarque)
        {
            return await _context.Produits.Where(u => u.MarqueProduit.MarqueId == idMarque).ToListAsync();
        }

        public async Task<IEnumerable<Produit>> SearchByNameAsync(string name)
        {
            return await _context.Produits.Where(u => u.NomProduit.ToUpper() == name.ToUpper()).ToListAsync();
        }
    }
}
