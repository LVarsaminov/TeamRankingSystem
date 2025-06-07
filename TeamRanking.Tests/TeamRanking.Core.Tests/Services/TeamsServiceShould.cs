using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Models;
using TeamRanking.Core.Repositories.Interfaces;
using TeamRanking.Core.Services.Implementations;
using TeamRanking.Core.Services.Interfaces;
using Xunit;

namespace TeamRanking.Core.Tests.Services
{
    public class TeamServiceShould
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRankingService> _rankingServiceMock;
        private readonly Mock<ITeamRepository> _teamRepositoryMock; 
        private readonly TeamService _teamService;

        public TeamServiceShould()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _rankingServiceMock = new Mock<IRankingService>();

            var teamsRepoMock = new Mock<ITeamRepository>();
            _unitOfWorkMock.Setup(u => u.Teams).Returns(teamsRepoMock.Object);

            _teamService = new TeamService(_unitOfWorkMock.Object, _mapperMock.Object, _rankingServiceMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedTeams()
        {
            // Arrange
            var teams = new List<Team> { new Team { Id = 1, Name = "Team1" } };
            _unitOfWorkMock.Setup(u => u.Teams.GetAllAsync()).ReturnsAsync(teams);
            var mappedTeams = new List<TeamDto> { new TeamDto { Id = 1, Name = "Team1" } };
            _mapperMock.Setup(m => m.Map<IEnumerable<TeamDto>>(teams)).Returns(mappedTeams);

            // Act
            var result = await _teamService.GetAllAsync();

            // Assert
            Assert.Equal(mappedTeams, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedTeam()
        {
            // Arrange
            var team = new Team { Id = 1, Name = "Team1" };
            _unitOfWorkMock.Setup(u => u.Teams.GetByIdAsync(1)).ReturnsAsync(team);
            var mappedTeam = new TeamDto { Id = 1, Name = "Team1" };
            _mapperMock.Setup(m => m.Map<TeamDto>(team)).Returns(mappedTeam);

            // Act
            var result = await _teamService.GetByIdAsync(1);

            // Assert
            Assert.Equal(mappedTeam, result);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnErrorMessage_WhenTeamExists()
        {
            // Arrange
            var teamDto = new CreateTeamDto { Name = "ExistingTeam" };
            _unitOfWorkMock.Setup(u => u.Teams.GetByNameAsync(teamDto.Name)).ReturnsAsync(new Team());

            // Act
            var (message, team) = await _teamService.CreateAsync(teamDto);

            // Assert
            Assert.Equal("This team already exists", message); 
            Assert.Null(team);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateTeam_WhenTeamDoesNotExist()
        {
            // Arrange
            var teamDto = new CreateTeamDto { Name = "NewTeam" };
            _unitOfWorkMock.Setup(u => u.Teams.GetByNameAsync(teamDto.Name)).ReturnsAsync((Team)null);

            var team = new Team { Name = "NewTeam" };
            _mapperMock.Setup(m => m.Map<Team>(teamDto)).Returns(team);

            _unitOfWorkMock.Setup(u => u.Teams.AddAsync(team)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var teamDtoResult = new TeamDto { Name = "NewTeam" };
            _mapperMock.Setup(m => m.Map<TeamDto>(team)).Returns(teamDtoResult);

            _rankingServiceMock.Setup(r => r.UpdateRankingsAsync()).Returns(Task.CompletedTask);

            // Act
            var (message, createdTeamDto) = await _teamService.CreateAsync(teamDto);

            // Assert
            Assert.True(string.IsNullOrEmpty(message));
            Assert.Equal(teamDtoResult, createdTeamDto);

            _unitOfWorkMock.Verify(u => u.Teams.AddAsync(team), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _rankingServiceMock.Verify(r => r.UpdateRankingsAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenTeamDoesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Teams.GetByIdAsync(1)).ReturnsAsync((Team)null);

            // Act
            var result = await _teamService.UpdateAsync(1, new UpdateTeamDto { Name = "UpdatedName" });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTeam_WhenTeamExists()
        {
            // Arrange
            var team = new Team { Id = 1, Name = "OldName" };
            _unitOfWorkMock.Setup(u => u.Teams.GetByIdAsync(1)).ReturnsAsync(team);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _rankingServiceMock.Setup(r => r.UpdateRankingsAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _teamService.UpdateAsync(1, new UpdateTeamDto { Name = "UpdatedName" });

            // Assert
            Assert.True(result);
            Assert.Equal("UpdatedName", team.Name);
            _unitOfWorkMock.Verify(u => u.Teams.Update(team), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _rankingServiceMock.Verify(r => r.UpdateRankingsAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenTeamDoesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Teams.GetByIdAsync(1)).ReturnsAsync((Team)null);

            // Act
            var result = await _teamService.DeleteAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTeam_WhenTeamExists()
        {
            // Arrange
            var team = new Team { Id = 1, Name = "TeamToDelete" };
            _unitOfWorkMock.Setup(u => u.Teams.GetByIdAsync(1)).ReturnsAsync(team);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _rankingServiceMock.Setup(r => r.UpdateRankingsAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _teamService.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.Teams.Delete(team), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _rankingServiceMock.Verify(r => r.UpdateRankingsAsync(), Times.Once);
        }
    }
}