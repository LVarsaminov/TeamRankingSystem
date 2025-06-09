using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Models;
using TeamRanking.Core.Repositories.Interfaces;
using TeamRanking.Core.Services.Implementations;
using TeamRanking.Core.Services.Strategies;
using Xunit;

namespace TeamRanking.Core.Tests.Services
{
    public class RankingServiceShould
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IScoringStrategy> _scoringStrategyMock;
        private readonly Mock<ITeamRepository> _teamRepositoryMock;
        private readonly Mock<IMatchRepository> _matchRepositoryMock;
        private readonly RankingService _rankingService;

        public RankingServiceShould()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _teamRepositoryMock = new Mock<ITeamRepository>();
            _matchRepositoryMock = new Mock<IMatchRepository>();

            _unitOfWorkMock.Setup(u => u.Teams).Returns(_teamRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Matches).Returns(_matchRepositoryMock.Object);

            _mapperMock = new Mock<IMapper>();
            _scoringStrategyMock = new Mock<IScoringStrategy>();

            _rankingService = new RankingService(_unitOfWorkMock.Object, _mapperMock.Object, _scoringStrategyMock.Object);
        }

        [Fact]
        public async Task GetRankingsAsync_ShouldReturnTeamsOrderedByPoints()
        {
            // Arrange
            var teams = new List<Team>
        {
            new Team { Id = 1, Name = "Team1" },
            new Team { Id = 2, Name = "Team2" }
        };
            var matches = new List<TeamRanking.Core.Models.Match>();

            _teamRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(teams);
            _matchRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(matches);

            var pointsDict = new Dictionary<int, int>
        {
            { 1, 10 },
            { 2, 20 }
        };
            _scoringStrategyMock.Setup(s => s.CalculatePoints(It.IsAny<List<Team>>(), It.IsAny<List<TeamRanking.Core.Models.Match>>()))
                .Returns(pointsDict);

            var teamDtos = new List<TeamDto>
        {
            new TeamDto { Id = 1, Name = "Team1" },
            new TeamDto { Id = 2, Name = "Team2" }
        };
            _mapperMock.Setup(m => m.Map<IEnumerable<TeamDto>>(teams)).Returns(teamDtos);

            // Act
            var result = await _rankingService.GetRankingsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal(20, resultList[0].Points); 
            Assert.Equal(10, resultList[1].Points);
        }

        [Fact]
        public async Task UpdateRankingsAsync_ShouldUpdateTeamsPointsAndSaveChanges()
        {
            // Arrange
            var teams = new List<Team>
        {
            new Team { Id = 1, Name = "Team1" },
            new Team { Id = 2, Name = "Team2" }
        };
            var matches = new List<TeamRanking.Core.Models.Match>();

            _teamRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(teams);
            _matchRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(matches);

            var pointsDict = new Dictionary<int, int>
        {
            { 1, 15 },
            { 2, 25 }
        };
            _scoringStrategyMock.Setup(s => s.CalculatePoints(It.IsAny<List<Team>>(), It.IsAny<List<TeamRanking.Core.Models.Match>>()))
                .Returns(pointsDict);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _rankingService.UpdateRankingsAsync();

            // Assert
            Assert.Equal(15, teams[0].Points);
            Assert.Equal(25, teams[1].Points);

            _teamRepositoryMock.Verify(r => r.Update(It.IsAny<Team>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateRankingsForCurrentMatchAsync_ShouldReturnIfNoMatches()
        {
            // Arrange
            var match = new TeamRanking.Core.Models.Match { Team1Id = 1, Team2Id = 2 };
            _teamRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Team>());
            _matchRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TeamRanking.Core.Models.Match>());

            // Act
            await _rankingService.UpdateRankingsForCurrentMatchAsync(match);

            // Assert
            _teamRepositoryMock.Verify(r => r.Update(It.IsAny<Team>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateRankingsForCurrentMatchAsync_ShouldUpdateTeamsPointsForMatch()
        {
            // Arrange
            var match = new TeamRanking.Core.Models.Match { Team1Id = 1, Team2Id = 2 };

            var teams = new List<Team>
        {
            new Team { Id = 1, Name = "Team1" },
            new Team { Id = 2, Name = "Team2" },
            new Team { Id = 3, Name = "Team3" }
        };
            var matches = new List<TeamRanking.Core.Models.Match> { match };

            _teamRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(teams);
            _matchRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(matches);

            var pointsDict = new Dictionary<int, int>
        {
            { 1, 18 },
            { 2, 22 },
            { 3, 5 }
        };
            _scoringStrategyMock.Setup(s => s.CalculatePoints(It.IsAny<List<Team>>(), It.IsAny<List<TeamRanking.Core.Models.Match>>()))
                .Returns(pointsDict);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _rankingService.UpdateRankingsForCurrentMatchAsync(match);

            // Assert
            var team1 = teams.First(t => t.Id == 1);
            var team2 = teams.First(t => t.Id == 2);

            Assert.Equal(18, team1.Points);
            Assert.Equal(22, team2.Points);

            _teamRepositoryMock.Verify(r => r.Update(team1), Times.Once);
            _teamRepositoryMock.Verify(r => r.Update(team2), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}