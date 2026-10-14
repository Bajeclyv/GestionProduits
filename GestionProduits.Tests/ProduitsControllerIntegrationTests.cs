using AutoMapper;
using GestionProduits.API.Controllers;
using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.EntityFramework;
using GestionProduits.API.Models.Repository;
using GestionProduits.API.Models.DataManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GestionProduits.Tests
{
    [TestClass]
    public sealed class ProduitsControllerIntegrationTests
    {
        private GestionProduitsContext _context = null!;
        private IProduitRepository _repository = null!;
        private ProduitsController _controller = null!;
        private IMapper _mapper = null!;

        private const string ConnectionString =
            "Host=localhost;Port=5432;" +
            "Database=produitsdb;" +
            "Username=postgres;Password=postgres";

        [TestInitialize]
        public async Task Initialize()
        {
            var options =
                new DbContextOptionsBuilder<GestionProduitsContext>()
                    .UseNpgsql(ConnectionString)
                    .Options;

            _context = new GestionProduitsContext(options);

            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
            
            var loggerFactory = NullLoggerFactory.Instance;
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, loggerFactory);

            mapperConfig.AssertConfigurationIsValid();
            _mapper = mapperConfig.CreateMapper();

            _repository = new ProduitManager(_context, _mapper);
            _controller = new ProduitsController(_repository);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (_context != null)
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.DisposeAsync();
            }
        }

        // Création des références nécessaires
        private async Task<(Marque marque, TypeProduit type)>
            SeedReferences()
        {
            var marque = new Marque
            {
                NomMarque = "MarqueTest"
            };

            var type = new TypeProduit
            {
                NomTypeProduit = "TypeTest"
            };

            _context.Marques.Add(marque);
            _context.TypeProduits.Add(type);

            await _context.SaveChangesAsync();

            return (marque, type);
        }

        // Création d'un produit de test
        private async Task<Produit> SeedProduit()
        {
            var (marque, type) = await SeedReferences();

            var produit = new Produit
            {
                NomProduit = "ProduitInitial",
                StockReel = 25,
                StockMin = 10,
                MarqueProduit = marque,
                Type = type
            };

            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            return produit;
        }

        // 1. Création puis relecture via les DTO
        [TestMethod]
        public async Task PostProduit_ThenGetProduit_PersistsProduct()
        {
            // Arrange
            await SeedReferences();

            var nouveauProduit = new ProduitDto
            {
                Nom = "ProduitIntegration",
                Marque = "MarqueTest",
                Type = "TypeTest"
            };

            // Act : création
            var postResult =
                await _controller.PostProduit(nouveauProduit);

            // Assert : HTTP 201
            var created =
                postResult.Result as CreatedAtActionResult;

            Assert.IsNotNull(created);
            Assert.AreEqual(201, created.StatusCode);

            // Récupération de l'identifiant généré en base
            var produitEnBase = await _context.Produits
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.NomProduit == "ProduitIntegration");

            Assert.IsNotNull(produitEnBase);
            Assert.IsTrue(produitEnBase.ProduitId > 0);

            int id = produitEnBase.ProduitId;

            // Act : relecture via le contrôleur
            var getResult = await _controller.GetById(id);

            // Assert : HTTP 200 et DTO
            var ok = getResult.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);

            var produitLu = ok.Value as ProduitDto;

            Assert.IsNotNull(produitLu);
            Assert.AreEqual(id, produitLu.Id);
            Assert.AreEqual(
                "ProduitIntegration",
                produitLu.Nom);
            Assert.AreEqual("MarqueTest", produitLu.Marque);
            Assert.AreEqual("TypeTest", produitLu.Type);
        }

        // 2. Modification d'un produit existant
        [TestMethod]
        public async Task PutProduit_ExistingProduct_UpdatesDatabase()
        {
            // Arrange
            var produit = await SeedProduit();

            var produitModifie = new ProduitDto
            {
                Id = produit.ProduitId,
                Nom = "ProduitModifie",
                Marque = "MarqueTest",
                Type = "TypeTest"
            };

            // Act
            var putResult = await _controller.PutProduit(
                produit.ProduitId,
                produitModifie);

            // Assert : HTTP 204
            Assert.IsInstanceOfType(
                putResult,
                typeof(NoContentResult));

            // Vérification directe en base
            var produitEnBase = await _context.Produits
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.ProduitId == produit.ProduitId);

            Assert.IsNotNull(produitEnBase);
            Assert.AreEqual(
                "ProduitModifie",
                produitEnBase.NomProduit);
            Assert.AreEqual(25, produitEnBase.StockReel);

            // Vérification via le contrôleur
            var getResult =
                await _controller.GetById(produit.ProduitId);

            var ok = getResult.Result as OkObjectResult;

            Assert.IsNotNull(ok);

            var produitLu = ok.Value as ProduitDto;

            Assert.IsNotNull(produitLu);
            Assert.AreEqual("ProduitModifie", produitLu.Nom);
        }

        // 3. Suppression d'un produit existant
        [TestMethod]
        public async Task DeleteProduit_ExistingProduct_ReturnsNoContent()
        {
            // Arrange
            var produit = await SeedProduit();
            int id = produit.ProduitId;

            // Act
            var deleteResult =
                await _controller.DeleteProduit(id);

            // Assert
            Assert.IsInstanceOfType(
                deleteResult,
                typeof(NoContentResult));

            // Vérification en base
            var produitEnBase = await _context.Produits
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProduitId == id);

            Assert.IsNull(produitEnBase);

            // Vérification via le contrôleur
            var getResult = await _controller.GetById(id);

            Assert.IsInstanceOfType(
                getResult.Result,
                typeof(NotFoundResult));
        }
    }
}