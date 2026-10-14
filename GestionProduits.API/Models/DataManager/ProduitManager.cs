using GestionProduits.API.Models.EntityFramework;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GestionProduits.API.Models.DTO;

namespace GestionProduits.API.Models.DataManager
{
    public class ProduitManager : IProduitRepository
    {
        private readonly GestionProduitsContext _context;
        private readonly IMapper _mapper;

        public ProduitManager(GestionProduitsContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ProduitDto>> GetAllAsync()
        {
            return await _context.Produits
                .ProjectTo<ProduitDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ProduitDto?> GetByIdAsync(int id)
        {
            return await _context.Produits
                .Where(p => p.ProduitId == id)
                .ProjectTo<ProduitDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
        
        public async Task AddAsync(ProduitDto dto)
        {
            var produit = _mapper.Map<Produit>(dto);

            await _context.Produits.AddAsync(produit);
        }

        public async Task UpdateAsync(ProduitDto produit1, ProduitDto produit2)
        {
            var produit = await _context.Produits
                .FirstOrDefaultAsync(p => p.ProduitId == produit1.Id);

            _mapper.Map(produit2, produit);
        }

        public Task DeleteAsync(ProduitDto produit)
        {
            var mappedProduit = _mapper.Map<Produit>(produit);
            _context.Produits.Remove(mappedProduit);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ProduitDetailDto>> GetAllWithDetailsAsync()
        {
            return await _context.Produits
                .ProjectTo<ProduitDetailDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ProduitDetailDto?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Produits
                .Where(p => p.ProduitId == id)
                .ProjectTo<ProduitDetailDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProduitDto>> GetByMarqueAsync(int idMarque)
        {
            return await _context.Produits
                .Where(p => p.MarqueProduit.MarqueId == idMarque)
                .ProjectTo<ProduitDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProduitDto>> SearchByNameAsync(string name)
        {
            return await _context.Produits
                .Where(p => p.NomProduit.Contains(name))
                .ProjectTo<ProduitDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
