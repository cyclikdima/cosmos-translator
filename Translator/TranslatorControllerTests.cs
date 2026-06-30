using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using Translator2.Controllers;
using Translator2.Services;
using Translator2.ViewModels;
using Translator2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Translator.Tests
{
    public class FakeCosmosService : CosmosService
    {
        public FakeCosmosService(CosmosClient client, IConfiguration config)
            : base(client, config) { }

        public override Task AddAsync(TranslationHistory item) => Task.CompletedTask;
        public override Task<List<TranslationHistory>> GetAllAsync() => Task.FromResult(new List<TranslationHistory>());
    }

    public class TranslatorControllerTests
    {
        private readonly Mock<TranslatorService> _mockService;
        private readonly CosmosService _fakeCosmos;

        public TranslatorControllerTests()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c[It.IsAny<string>()]).Returns("TestDb");

            _mockService = new Mock<TranslatorService>(mockConfig.Object);

            var mockClient = new Mock<CosmosClient>();
            var mockDatabase = new Mock<Database>();
            var mockContainer = new Mock<Container>();

            mockClient.Setup(x => x.GetDatabase(It.IsAny<string>())).Returns(mockDatabase.Object);
            mockDatabase.Setup(x => x.GetContainer(It.IsAny<string>())).Returns(mockContainer.Object);

            _fakeCosmos = new FakeCosmosService(mockClient.Object, mockConfig.Object);
        }

        [Fact]
        public async Task Index_Post_WithEmptyText_ReturnsSameViewImmediately()
        {
            var controller = new TranslatorController(_mockService.Object, _fakeCosmos);
            var model = new TranslatorViewModel { Text = "   " };

            var result = await controller.Index(model) as ViewResult;
            var resultModel = result?.Model as TranslatorViewModel;

            Assert.NotNull(result);
            Assert.Null(resultModel?.Result);
        }

        [Fact]
        public async Task Change_Post_WithEmptyResult_ReturnsIndexViewWithoutCrashing()
        {
            var controller = new TranslatorController(_mockService.Object, _fakeCosmos);
            var model = new TranslatorViewModel { Result = "" };


            var result = await controller.Change(model) as ViewResult;

            Assert.Equal("Index", result?.ViewName);
        }
    }
}
