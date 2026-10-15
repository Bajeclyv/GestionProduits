using System.Reflection;
using AutoMapper;
using GestionProduits.API.Controllers;
using GestionProduits.API.Models.DataManager;
using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.EntityFramework;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionProduits.Tests
{
    [TestClass]
    public class ProduitsControllerIntegrationTests
    {
        private const string ConnectionString =
            "Host=localhost;Port=5432;Database=produitsdb;Username=postgres;Password=postgres";

        private GestionProduitsContext _context = null!;
        private IMapper _mapper = null!;
        private ProduitManager _repository = null!;
        private ProduitsController _controller = null!;

        [TestInitialize]
        public async Task Initialize()
        {
            var options = new DbContextOptionsBuilder<GestionProduitsContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            _context = new GestionProduitsContext(options);

            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            var mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(typeof(ProduitManager).Assembly);
                }, NullLoggerFactory.Instance
            );

            _mapper = mapperConfiguration.CreateMapper();
            _repository = new ProduitManager(_context, _mapper);
            _controller = new ProduitsController(
                _repository);
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

        [TestMethod]
        public async Task PostProduit_ValidProduct_ReturnsCreatedAtActionAndPersists()
        {
            // Arrange
            var marque = await SeedAsync(CreateEntity<Marque>(
                ("Nom", "Marque Integration")));

            var typeProduit = await SeedAsync(CreateEntity<TypeProduit>(
                ("Nom", "Type Integration")));

            var produitDto = CreateProduitDto(
                nom: "Produit Integration",
                stock: 25,
                stockMin: 10,
                marqueId: GetEntityId(marque),
                typeId: GetEntityId(typeProduit));

            // Act
            var postResult = await _controller.PostProduit(produitDto);

            // Assert
            Assert.IsInstanceOfType(postResult, typeof(CreatedAtActionResult));

            var createdResult = (CreatedAtActionResult)postResult.Result;
            var createdId = GetRouteId(createdResult);

            Assert.IsTrue(createdId > 0);

            var produitEnBase = await _context.Set<Produit>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => GetEntityId(p) == createdId);

            Assert.IsNotNull(produitEnBase);
            Assert.AreEqual("Produit Integration", GetValue<string?>(produitEnBase, "Nom", "NomProduit"));
            Assert.AreEqual(25, GetValue<int>(produitEnBase, "Stock", "StockReel"));

            // Lecture immédiate via le contrôleur
            var getResult = await _controller.GetById(createdId);

            Assert.IsNotNull(getResult.Result);
            Assert.IsInstanceOfType(getResult.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task PutProduit_ExistingProduct_ReturnsNoContentAndPersistsUpdate()
        {
            // Arrange
            var marque = await SeedAsync(CreateEntity<Marque>(
                ("Nom", "Marque Update")));

            var typeProduit = await SeedAsync(CreateEntity<TypeProduit>(
                ("Nom", "Type Update")));

            var produit = await SeedAsync(CreateEntity<Produit>(
                ("Nom", "ProduitInitial"),
                ("NomProduit", "ProduitInitial"),
                ("Stock", 25),
                ("StockReel", 25),
                ("StockMin", 10),
                ("MarqueProduit", marque),
                ("Type", typeProduit)));

            var produitId = GetEntityId(produit);

            var produitModifie = CreateProduitDto(
                nom: "ProduitModifie",
                stock: 40,
                stockMin: 10,
                marqueId: GetEntityId(marque),
                typeId: GetEntityId(typeProduit),
                produitId: produitId);

            // Act
            var putResult = await _controller.PutProduit(produitId, produitModifie);

            // Assert
            Assert.IsInstanceOfType(putResult, typeof(NoContentResult));

            var produitEnBase = await _context.Set<Produit>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => GetEntityId(p) == produitId);

            Assert.IsNotNull(produitEnBase);
            Assert.AreEqual("ProduitModifie", GetValue<string?>(produitEnBase, "Nom", "NomProduit"));
            Assert.AreEqual(40, GetValue<int>(produitEnBase, "Stock", "StockReel"));

            var getResult = await _controller.GetById(produitId);

            Assert.IsNotNull(getResult.Result);
            Assert.IsInstanceOfType(getResult.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task DeleteProduit_ExistingProduct_ReturnsNoContent()
        {
            // Arrange
            var marque = await SeedAsync(CreateEntity<Marque>(
                ("Nom", "Marque Delete")));

            var typeProduit = await SeedAsync(CreateEntity<TypeProduit>(
                ("Nom", "Type Delete")));

            var produit = await SeedAsync(CreateEntity<Produit>(
                ("Nom", "ProduitDelete"),
                // ("NomProduit", "ProduitDelete"),
                ("Stock", 25),
                ("StockReel", 25),
                ("StockMin", 10),
                ("MarqueProduit", marque),
                ("Type", typeProduit)));

            var id = GetEntityId(produit);

            // Act
            var deleteResult = await _controller.DeleteProduit(id);

            // Assert
            Assert.IsInstanceOfType(deleteResult, typeof(NoContentResult));

            var produitEnBase = await _context.Set<Produit>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => GetEntityId(p) == id);

            Assert.IsNull(produitEnBase);

            var getResult = await _controller.GetById(id);

            Assert.IsInstanceOfType(getResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostProduit_WithMissingMarqueId_ThrowsDbUpdateException()
        {
            // Arrange
            var typeProduit = await SeedAsync(CreateEntity<TypeProduit>(
                ("Nom", "Type Exception")));

            var produitDto = CreateProduitDto(
                nom: "ProduitErreur",
                stock: 10,
                stockMin: 2,
                marqueId: 999_999,
                typeId: GetEntityId(typeProduit));

            // Act + Assert
            DbUpdateException? exception = null;

            try
            {
                await _controller.PostProduit(produitDto);
            }
            catch (DbUpdateException ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception, "Une DbUpdateException était attendue.");
        }

        private async Task<TEntity> SeedAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        private static TEntity CreateEntity<TEntity>(params (string Name, object? Value)[] values)
            where TEntity : new()
        {
            var entity = new TEntity();

            foreach (var (name, value) in values)
            {
                SetProperty(entity, name, value);
            }

            return entity;
        }

        private static ProduitDto CreateProduitDto(
            string nom,
            int stock,
            int stockMin,
            int marqueId,
            int typeId,
            int? produitId = null)
        {
            var dto = new ProduitDto();

            SetProperty(dto, "ProduitId", produitId);
            SetProperty(dto, "Id", produitId);

            SetProperty(dto, "Nom", nom);
            SetProperty(dto, "NomProduit", nom);

            SetProperty(dto, "Stock", stock);
            SetProperty(dto, "StockReel", stock);

            SetProperty(dto, "StockMin", stockMin);

            SetProperty(dto, "IdMarque", marqueId);
            SetProperty(dto, "MarqueId", marqueId);
            SetProperty(dto, "IdMarqueProduit", marqueId);

            SetProperty(dto, "IdType", typeId);
            SetProperty(dto, "TypeId", typeId);
            SetProperty(dto, "IdTypeProduit", typeId);
            SetProperty(dto, "TypeProduitId", typeId);

            return dto;
        }

        private static void SetProperty(object target, string propertyName, object? value)
        {
            var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (property == null || !property.CanWrite)
            {
                return;
            }

            property.SetValue(target, value);
        }

        private static int GetRouteId(CreatedAtActionResult result)
        {
            if (result.RouteValues != null)
            {
                if (result.RouteValues.TryGetValue("id", out var idValue) && idValue != null)
                {
                    return Convert.ToInt32(idValue);
                }

                if (result.RouteValues.TryGetValue("produitId", out var produitIdValue) && produitIdValue != null)
                {
                    return Convert.ToInt32(produitIdValue);
                }

                if (result.RouteValues.TryGetValue("produitid", out var produitidValue) && produitidValue != null)
                {
                    return Convert.ToInt32(produitidValue);
                }
            }

            throw new AssertFailedException("Impossible de récupérer l'identifiant du produit créé.");
        }

        private static int GetEntityId<TEntity>(TEntity entity)
        {
            var type = entity!.GetType();

            var idProperty = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p =>
                    p.PropertyType == typeof(int) &&
                    (string.Equals(p.Name, $"{type.Name}Id", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase)));

            if (idProperty == null)
            {
                throw new AssertFailedException(
                    $"Impossible de déterminer la clé primaire pour {type.Name}.");
            }

            return (int)(idProperty.GetValue(entity) ?? 0);
        }

        private static T GetValue<T>(object target, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

                if (property != null && property.CanRead)
                {
                    var value = property.GetValue(target);
                    if (value is T typedValue)
                    {
                        return typedValue;
                    }
                }
            }

            throw new AssertFailedException(
                $"Aucune des propriétés {string.Join(", ", propertyNames)} n'a pu être lue sur {target.GetType().Name}.");
        }
    }
}