using GestionProduits.API.Controllers;
using GestionProduits.API.Models.DTO;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GestionProduits.Tests;

[TestClass]
public class CatalogueControllersTests
{
    [TestMethod]
    public async Task MarquesController_GetAll_ReturnsBrandsFromRepository()
    {
        var repository = new Mock<IDataRepository<MarqueDto>>();
        var expected = new[] { new MarqueDto { Id = 3, Nom = "Acme", NbProduits = 2 } };
        repository.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);
        var controller = new MarquesController(repository.Object);

        var result = await controller.GetAll();

        var ok = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        Assert.AreSame(expected, ok.Value);
        repository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [TestMethod]
    public async Task MarquesController_Post_SavesBrandAndReturnsCreated()
    {
        var repository = new Mock<IDataRepository<MarqueDto>>();
        var brand = new MarqueDto { Id = 12, Nom = "Acme" };
        repository.Setup(r => r.AddAsync(brand)).Returns(Task.CompletedTask);
        repository.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        var controller = new MarquesController(repository.Object);

        var result = await controller.Post(brand);

        var created = Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
        Assert.AreEqual(201, created.StatusCode);
        Assert.AreSame(brand, created.Value);
        repository.Verify(r => r.AddAsync(brand), Times.Once);
        repository.Verify(r => r.SaveAsync(), Times.Once);
    }

    [TestMethod]
    public async Task TypesProduitsController_GetById_UnknownId_ReturnsNotFound()
    {
        var repository = new Mock<IDataRepository<TypeProduitDto>>();
        repository.Setup(r => r.GetByIdAsync(47)).ReturnsAsync((TypeProduitDto?)null);
        var controller = new TypesProduitsController(repository.Object);

        var result = await controller.GetById(47);

        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
        repository.Verify(r => r.GetByIdAsync(47), Times.Once);
    }

    [TestMethod]
    public async Task TypesProduitsController_Delete_ExistingType_ReturnsNoContentAndSaves()
    {
        var repository = new Mock<IDataRepository<TypeProduitDto>>();
        var productType = new TypeProduitDto { Id = 5, Nom = "Accessoire" };
        repository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(productType);
        repository.Setup(r => r.DeleteAsync(productType)).Returns(Task.CompletedTask);
        repository.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        var controller = new TypesProduitsController(repository.Object);

        var result = await controller.Delete(5);

        Assert.IsInstanceOfType<NoContentResult>(result);
        repository.Verify(r => r.DeleteAsync(productType), Times.Once);
        repository.Verify(r => r.SaveAsync(), Times.Once);
    }
}
