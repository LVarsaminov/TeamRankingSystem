using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamRanking.API.Controllers;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Services.Interfaces;

namespace TeamRanking.API.Tests.Controllers
{
    public class TeamsControllerShould
    {
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly TeamsController _controller;
        private readonly Mock<IRankingService> _mockRankingService;

        public TeamsControllerShould()
        {
            _mockTeamService = new Mock<ITeamService>();
            _mockRankingService = new Mock<IRankingService>();
            _controller = new TeamsController(_mockTeamService.Object, _mockRankingService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithListOfTeams()
        {
            // Arrange
            var teams = new List<TeamDto> { new TeamDto { Id = 1 }, new TeamDto { Id = 2 } };
            _mockTeamService.Setup(s => s.GetAllAsync()).ReturnsAsync(teams);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(teams, okResult.Value);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOk()
        {
            var team = new TeamDto { Id = 1 };
            _mockTeamService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(team);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(team, okResult.Value);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            _mockTeamService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((TeamDto)null);

            var result = await _controller.GetById(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("This team does not exist!", notFoundResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsTeamDto_WhenMessageIsEmpty()
        {
            var createDto = new CreateTeamDto();
            var teamDto = new TeamDto { Id = 1 };

            _mockTeamService.Setup(s => s.CreateAsync(createDto))
                .ReturnsAsync((message: "", Team: teamDto));

            var result = await _controller.Create(createDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(teamDto, okResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsNotFound_WhenMessageIsNotEmpty()
        {
            var createDto = new CreateTeamDto();

            _mockTeamService.Setup(s => s.CreateAsync(createDto))
                .ReturnsAsync((message: "Team already exists", Team: null));

            var result = await _controller.Create(createDto);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Team already exists", notFoundResult.Value);
        }

        [Fact]
        public async Task Update_ExistingTeam_ReturnsOk()
        {
            var updateDto = new UpdateTeamDto();
            _mockTeamService.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(true);

            var result = await _controller.Update(1, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Updated successfuly!", okResult.Value);
        }

        [Fact]
        public async Task Update_NonExistingTeam_ReturnsNotFound()
        {
            var updateDto = new UpdateTeamDto();
            _mockTeamService.Setup(s => s.UpdateAsync(1, updateDto)).ReturnsAsync(false);

            var result = await _controller.Update(1, updateDto);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("This team does not exist!", notFoundResult.Value);
        }

        [Fact]
        public async Task Delete_ExistingTeam_ReturnsNoContent()
        {
            _mockTeamService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistingTeam_ReturnsNotFound()
        {
            _mockTeamService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundResult>(result);
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