using GestionProduits.API.Controllers;
using GestionProduits.API.Models.DataManager;
using GestionProduits.API.Models.EntityFramework;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Npgsql;

namespace GestionProduits.Tests
{
    [TestClass]
    public sealed class ProduitsControllerIntegrationTests
    {
        private GestionProduitsContext _context = null!;
        private IProduitRepository _repository = null!;
        private ProduitsController _controller = null!;
        private const string ConnectionString =
            "Host=localhost;Port=5432;" +
            "Database=produitsdb;" +
            "Username=postgres;Password=postgres";

        [TestInitialize]
        public async Task Initialize()
        {
            var options = new DbContextOptionsBuilder<GestionProduitsContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            _context = new GestionProduitsContext(options);

            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            _repository = new ProduitManager(_context);
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

        // Helpers : création des entités de référence

        private async Task<(Marque marque, TypeProduit type)> SeedReferences()
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

        private async Task<Produit> SeedProduit()
        {
            var (marque, type) = await SeedReferences();

            var produit = new Produit
            {
                NomProduit = "ProduitInitial",
                StockReel = 25,
                MarqueProduit = new Marque { MarqueId = marque.MarqueId },
                Type = new TypeProduit { TypeProduitId = type.TypeProduitId }
            };

            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            return produit;
        }

        // 1. Création puis relecture complète

        [TestMethod]
        public async Task PostProduit_ThenGetProduit_PersistsProduct()
        {
            // Arrange
            var (marque, type) = await SeedReferences();

            var nouveauProduit = new Produit
            {
                NomProduit = "ProduitIntegration",
                StockReel = 25,
                MarqueProduit = new Marque { MarqueId = marque.MarqueId },
                Type = new TypeProduit { TypeProduitId = type.TypeProduitId }
            };

            // Act : création
            var postResult =
                await _controller.PostProduit(nouveauProduit);

            // Assert : HTTP 201
            var created = postResult.Result as CreatedAtActionResult;

            Assert.IsNotNull(created);
            Assert.AreEqual(201, created.StatusCode);

            // Récupérer l'identifiant généré par PostgreSQL
            Assert.IsTrue(nouveauProduit.ProduitId > 0);
            int id = nouveauProduit.ProduitId;

            // Act : relecture par le contrôleur
            var getResult = await _controller.GetById(id);

            // Assert : HTTP 200
            Assert.IsNotNull(getResult.Result);

            var ok = getResult.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);

            var produitLu = ok.Value as Produit;

            Assert.IsNotNull(produitLu);
            Assert.AreEqual(id, produitLu.ProduitId);
            Assert.AreEqual(
                "ProduitIntegration",
                produitLu.NomProduit
            );
            Assert.AreEqual(25, produitLu.StockReel);
        }

        // 2. Mise à jour réelle

        [TestMethod]
        public async Task PutProduit_ExistingProduct_UpdatesDatabase()
        {
            // Arrange
            var produit = await SeedProduit();

            produit.NomProduit = "ProduitModifie";
            produit.StockReel = 50;

            // Act
            var putResult = await _controller.PutProduit(
                produit.ProduitId,
                produit
            );

            // Assert : HTTP 204
            Assert.IsInstanceOfType(
                putResult,
                typeof(NoContentResult)
            );

            // Relire avec le contrôleur
            var getResult =
                await _controller.GetById(produit.ProduitId);

            Assert.IsNotNull(getResult.Result);

            var ok = getResult.Result as OkObjectResult;

            Assert.IsNotNull(ok);

            var produitLu = ok.Value as Produit;

            Assert.IsNotNull(produitLu);
            Assert.AreEqual("ProduitModifie", produitLu.NomProduit);
            Assert.AreEqual(50, produitLu.StockReel);
        }

        [TestMethod]
        public async Task DeleteProduit_ExistingProduct_ReturnsNoContent()
        {
            // Arrange
            var produit = await SeedProduit();
            int id = produit.ProduitId;

            // Act
            var deleteResult = await _controller.DeleteProduit(id);

            // Assert
            Assert.IsInstanceOfType(
                deleteResult,
                typeof(NoContentResult)
            );

            var getResult = await _controller.GetById(id);

            Assert.IsNotNull(getResult.Result);

            Assert.IsInstanceOfType(
                getResult.Result,
                typeof(NotFoundResult)
            );
        }
    }
}
