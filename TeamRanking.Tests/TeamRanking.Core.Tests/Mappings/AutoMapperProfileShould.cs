using AutoMapper;
using System;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Mappings;
using TeamRanking.Core.Models;
using Xunit;

namespace TeamRanking.Core.Tests.Mappings
{
    public class AutoMapperProfileShould
    {
        private readonly IMapper _mapper;

        public AutoMapperProfileShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });


            _mapper = config.CreateMapper();
        }

        [Fact]
        public void Should_Map_Match_To_MatchDto_Correctly()
        {
            // Arrange
            var match = new Match
            {
                Team1 = new Team { Name = "Team One" },
                Team2 = new Team { Name = "Team Two" },
                Team1Score = 3,
                Team2Score = 2,
                MatchDate = new DateTime(2025, 6, 7)
            };

            // Act
            var matchDto = _mapper.Map<MatchDto>(match);

            // Assert
            Assert.Equal(match.Team1.Name, matchDto.Team1Name);
            Assert.Equal(match.Team2.Name, matchDto.Team2Name);
            Assert.Equal(match.Team1Score, matchDto.Team1Score);
            Assert.Equal(match.Team2Score, matchDto.Team2Score);
            Assert.Equal(match.MatchDate, matchDto.MatchDate);
        }

        [Fact]
        public void Should_Map_CreateMatchDto_To_Match_Correctly()
        {
            // Arrange
            var createDto = new CreateMatchDto
            {
                Team1Name = "Team One",
                Team2Name = "Team Two",
                Team1Score = 4,
                Team2Score = 1,
                MatchDate = new DateTime(2025, 6, 7)
            };

            // Act
            var match = _mapper.Map<Match>(createDto);

            // Assert
            Assert.Equal(createDto.Team1Name, match.Team1.Name);
            Assert.Equal(createDto.Team2Name, match.Team2.Name);
            Assert.Equal(createDto.Team1Score, match.Team1Score);
            Assert.Equal(createDto.Team2Score, match.Team2Score);
            Assert.Equal(createDto.MatchDate, match.MatchDate);
        }

        [Fact]
        public void Should_Map_UpdateMatchDto_To_Match_Conditionally()
        {
            // Arrange
            var updateDto = new UpdateMatchDto
            {
                Team1Score = 5,
                Team2Score = 0,
                MatchDate = new DateTime(2025, 6, 8)
            };

            var match = new Match
            {
                Team1Score = 3,
                Team2Score = 2,
                MatchDate = new DateTime(2025, 6, 7)
            };

            // Act
            _mapper.Map(updateDto, match);

            // Assert
            Assert.Equal(5, match.Team1Score);
            Assert.Equal(0, match.Team2Score);
            Assert.Equal(updateDto.MatchDate, match.MatchDate);
        }

        [Fact]
        public void Should_Map_Team_To_TeamDto()
        {
            // Arrange
            var team = new Team { Id = 1, Name = "Team A" };

            // Act
            var dto = _mapper.Map<TeamDto>(team);

            // Assert
            Assert.Equal(team.Id, dto.Id);
            Assert.Equal(team.Name, dto.Name);
        }

        [Fact]
        public void Should_Map_CreateTeamDto_To_Team()
        {
            // Arrange
            var createDto = new CreateTeamDto { Name = "New Team" };

            // Act
            var team = _mapper.Map<Team>(createDto);

            // Assert
            Assert.Equal(createDto.Name, team.Name);
        }

        [Fact]
        public void Should_Map_UpdateTeamDto_To_Team_Conditionally()
        {
            // Arrange
            var updateDto = new UpdateTeamDto { Name = "Updated Team" };
            var team = new Team { Name = "Old Team" };

            // Act
            _mapper.Map(updateDto, team);

            // Assert
            Assert.Equal("Updated Team", team.Name);
        }
    }
}