using GestionProduits.API.Controllers;
using GestionProduits.API.Models.EntityFramework;
using GestionProduits.API.Models.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GestionProduits.Tests
{
    [TestClass]
    public sealed class ProduitsControllerTests
    {
        private Mock<IProduitRepository> _mockRepository = null!;
        private ProduitsController _controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new Mock<IProduitRepository>();

            _controller = new ProduitsController(_mockRepository.Object);
        }

        [TestMethod]
        public async Task GetProduit_ExistingId_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var produit = new Produit { ProduitId = id };


            _mockRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(produit);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsInstanceOfType<OkObjectResult>(result.Result);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreSame(produit, okResult.Value);

            _mockRepository.Verify(
                r => r.GetByIdAsync(id),
                Times.Once
            );
        }

        [TestMethod]
        public async Task GetProduit_UnknownId_ReturnsNotFoundResult()
        {
            // Arrange
            int id = 999;

            _mockRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Produit?)null);

            // Act
            var result = await _controller.GetById(id);
            // var okResult = (NotFoundResult)result.Result;

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(result.Result);
        }

        [TestMethod]
        public async Task PostProduit_ValidProduct_ReturnsCreatedAtAction()
        {
            // Arrange
            var produit = new Produit { ProduitId = 1 };

            _mockRepository
                .Setup(r => r.AddAsync(produit))
                .Returns(Task.CompletedTask);

            _mockRepository
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostProduit(produit);

            // Assert
            var createdResult =
                Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);

            Assert.AreEqual(201, createdResult.StatusCode);

            _mockRepository.Verify(
                r => r.AddAsync(produit),
                Times.Once
            );

            _mockRepository.Verify(
                r => r.SaveAsync(),
                Times.Once
            );
        }

        [TestMethod]
        public async Task PutProduit_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            int id = 1;
            var produit = new Produit { ProduitId = 2 };

            // Act
            var result = await _controller.PutProduit(id, produit);

            // Assert
            Assert.IsInstanceOfType<BadRequestObjectResult>(result);

            _mockRepository.Verify(
                r => r.SaveAsync(),
                Times.Never
            );

            _mockRepository.Verify(
                r => r.UpdateAsync(produit, It.IsAny<Produit>()),
                Times.Never
            );
        }

        [TestMethod]
        public async Task DeleteProduit_ExistingId_ReturnsNoContent()
        {
            // Arrange
            int id = 1;
            var produit = new Produit { ProduitId = id };

            _mockRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(produit);

            _mockRepository
                .Setup(r => r.DeleteAsync(produit))
                .Returns(Task.CompletedTask);

            _mockRepository
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteProduit(id);

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(result);

            _mockRepository.Verify(
                r => r.DeleteAsync(produit),
                Times.Once
            );

            _mockRepository.Verify(
                r => r.SaveAsync(),
                Times.Once
            );
        }
    }
}
