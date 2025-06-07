using Xunit;
using Moq;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Core.Models;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Services.Implementations;
using TeamRanking.Core.Repositories.Interfaces;
using TeamRanking.Core.Services.Interfaces;

namespace TeamRanking.Core.Tests.Services
{
    public class MatchServiceShould
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRankingService> _rankingServiceMock;
        private readonly Mock<ITeamRepository> _teamRepositoryMock;
        private readonly MatchService _service;

        public MatchServiceShould()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _rankingServiceMock = new Mock<IRankingService>();
            _teamRepositoryMock = new Mock<ITeamRepository>();

            _service = new MatchService(_unitOfWorkMock.Object, _mapperMock.Object, _rankingServiceMock.Object, _teamRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedMatchDtos()
        {
            // Arrange
            var matches = new List<Match> { new Match(), new Match() };
            var matchDtos = new List<MatchDto> { new MatchDto(), new MatchDto() };

            _unitOfWorkMock.Setup(u => u.Matches.GetAllAsync()).ReturnsAsync(matches);
            _mapperMock.Setup(m => m.Map<IEnumerable<MatchDto>>(matches)).Returns(matchDtos);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(matchDtos, result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMappedMatchDto()
        {
            // Arrange
            var match = new Match { Id = 1 };
            var matchDto = new MatchDto { Id = 1 };

            _unitOfWorkMock.Setup(u => u.Matches.GetByIdAsync(1)).ReturnsAsync(match);
            _mapperMock.Setup(m => m.Map<MatchDto>(match)).Returns(matchDto);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.Equal(matchDto, result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsErrorMessage_WhenTeam1DoesNotExist()
        {
            // Arrange
            var createDto = new CreateMatchDto { Team1Name = "TeamA", Team2Name = "TeamB" };
            _teamRepositoryMock.Setup(t => t.GetByNameAsync("TeamA")).ReturnsAsync((Team)null);

            // Act
            var (message, matchDto) = await _service.CreateAsync(createDto);

            // Assert
            Assert.Equal("TeamA does not exist!", message);
            Assert.Null(matchDto);
        }

        [Fact]
        public async Task CreateAsync_ReturnsErrorMessage_WhenTeam2DoesNotExist()
        {
            // Arrange
            var createDto = new CreateMatchDto { Team1Name = "TeamA", Team2Name = "TeamB" };
            var teamA = new Team { Id = 1, PlayedMatchesCount = 0 };

            _teamRepositoryMock.Setup(t => t.GetByNameAsync("TeamA")).ReturnsAsync(teamA);
            _teamRepositoryMock.Setup(t => t.GetByNameAsync("TeamB")).ReturnsAsync((Team)null);

            // Act
            var (message, matchDto) = await _service.CreateAsync(createDto);

            // Assert
            Assert.Equal("TeamB does not exist!", message);
            Assert.Null(matchDto);
        }

        [Fact]
        public async Task CreateAsync_CreatesMatchAndUpdatesRankings()
        {
            // Arrange
            var createDto = new CreateMatchDto { Team1Name = "TeamA", Team2Name = "TeamB" };
            var teamA = new Team { Id = 1, PlayedMatchesCount = 0 };
            var teamB = new Team { Id = 2, PlayedMatchesCount = 0 };
            var match = new Match();
            var matchDto = new MatchDto();

            _teamRepositoryMock.Setup(t => t.GetByNameAsync("TeamA")).ReturnsAsync(teamA);
            _teamRepositoryMock.Setup(t => t.GetByNameAsync("TeamB")).ReturnsAsync(teamB);
            _mapperMock.Setup(m => m.Map<Match>(createDto)).Returns(match);
            _mapperMock.Setup(m => m.Map<MatchDto>(match)).Returns(matchDto);

            _unitOfWorkMock.Setup(u => u.Matches.AddAsync(match)).Returns(Task.CompletedTask);
            _rankingServiceMock.Setup(r => r.UpdateRankingsForCurrentMatchAsync(match)).Returns(Task.CompletedTask);

            // Act
            var (message, resultDto) = await _service.CreateAsync(createDto);

            // Assert
            Assert.Equal(string.Empty, message);
            Assert.Equal(matchDto, resultDto);
            Assert.Equal(1, teamA.PlayedMatchesCount);
            Assert.Equal(1, teamB.PlayedMatchesCount);
            Assert.Equal(teamA.Id, match.Team1Id);
            Assert.Equal(teamB.Id, match.Team2Id);
            Assert.Equal(teamA, match.Team1);
            Assert.Equal(teamB, match.Team2);

            _unitOfWorkMock.Verify(u => u.Matches.AddAsync(match), Times.Once);
            _rankingServiceMock.Verify(r => r.UpdateRankingsForCurrentMatchAsync(match), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenMatchDoesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Matches.ExistsAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _service.UpdateAsync(1, new UpdateMatchDto());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesMatch_WhenExists()
        {
            // Arrange
            var updateDto = new UpdateMatchDto();
            var match = new Match { Id = 1 };

            _unitOfWorkMock.Setup(u => u.Matches.ExistsAsync(1)).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<Match>(updateDto)).Returns(match);

            // Act
            var result = await _service.UpdateAsync(1, updateDto);

            // Assert
            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.Matches.Update(match), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal(1, match.Id);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenMatchNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Matches.GetByIdAsync(1)).ReturnsAsync((Match)null);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_DeletesMatchAndDecrementsPlayedMatchesCount()
        {
            // Arrange
            var match = new Match
            {
                Id = 1,
                Team1 = new Team { PlayedMatchesCount = 2 },
                Team2 = new Team { PlayedMatchesCount = 3 }
            };

            _unitOfWorkMock.Setup(u => u.Matches.GetByIdAsync(1)).ReturnsAsync(match);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _unitOfWorkMock.Verify(u => u.Matches.Delete(match), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal(1, match.Team1.PlayedMatchesCount);
            Assert.Equal(2, match.Team2.PlayedMatchesCount);
        }
    }
}