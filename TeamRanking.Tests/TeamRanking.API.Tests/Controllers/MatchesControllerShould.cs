using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TeamRanking.API.Controllers;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Services.Interfaces;

namespace TeamRanking.API.Tests.Controllers
{
    public class MatchesControllerShould
    {
        private readonly Mock<IMatchService> _mockMatchService;
        private readonly Mock<IRankingService> _mockRankingService;
        private readonly MatchesController _controller;

        public MatchesControllerShould()
        {
            _mockMatchService = new Mock<IMatchService>();
            _mockRankingService = new Mock<IRankingService>();
            _controller = new MatchesController(_mockMatchService.Object, _mockRankingService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithListOfMatches()
        {
            // Arrange
            var matches = new List<MatchDto> { new MatchDto { Id = 1 }, new MatchDto { Id = 2 } };
            _mockMatchService.Setup(s => s.GetAllAsync()).ReturnsAsync(matches);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(matches, okResult.Value);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            // Arrange
            var match = new MatchDto { Id = 1 };
            _mockMatchService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(match);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(match, okResult.Value);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            _mockMatchService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((MatchDto)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsMatchDto_WhenMessageIsEmpty()
        {
            var createDto = new CreateMatchDto();
            var matchDto = new MatchDto { Id = 1 };
            _mockMatchService.Setup(s => s.CreateAsync(createDto))
                             .ReturnsAsync((null, matchDto));

            var result = await _controller.Create(createDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(matchDto, okResult.Value);
            _mockRankingService.Verify(r => r.UpdateRankingsAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnsMessage_WhenMessageIsNotEmpty()
        {
            var createDto = new CreateMatchDto();
            _mockMatchService.Setup(s => s.CreateAsync(createDto))
                             .ReturnsAsync(("Match already exists", null));

            var result = await _controller.Create(createDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Match already exists", okResult.Value);
            _mockRankingService.Verify(r => r.UpdateRankingsAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_ExistingMatch_ReturnsNoContent()
        {
            var updateDto = new UpdateMatchDto();
            _mockMatchService.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(true);

            var result = await _controller.Update(1, updateDto);

            Assert.IsType<NoContentResult>(result);
            _mockRankingService.Verify(r => r.UpdateRankingsAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_NonExistingMatch_ReturnsNotFound()
        {
            var updateDto = new UpdateMatchDto();
            _mockMatchService.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(false);

            var result = await _controller.Update(1, updateDto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingMatch_ReturnsOk()
        {
            _mockMatchService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted!", okResult.Value);
            _mockRankingService.Verify(r => r.UpdateRankingsAsync(), Times.Once);
        }

        [Fact]
        public async Task Delete_NonExistingMatch_ReturnsNotFound()
        {
            _mockMatchService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _controller.Delete(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Match with Id:99 does not exist!", notFoundResult.Value);
        }

        [Fact]
        public async Task GetRankings_ReturnsOk_WithListOfTeams()
        {
            var rankings = new List<TeamDto> { new TeamDto { Id = 1 }, new TeamDto { Id = 2 } };
            _mockRankingService.Setup(s => s.GetRankingsAsync()).ReturnsAsync(rankings);

            var result = await _controller.GetRankings();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(rankings, okResult.Value);
        }
    }
}