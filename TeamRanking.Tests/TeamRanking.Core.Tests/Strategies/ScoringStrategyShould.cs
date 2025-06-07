using System.Collections.Generic;
using System.Linq;
using TeamRanking.Core.Models;
using TeamRanking.Core.Services.Strategies;
using Xunit;

namespace TeamRanking.Core.Tests.Strategies
{
    public class ScoringStrategyShould
    {
        private readonly ScoringStrategy _scoringStrategy;

        public ScoringStrategyShould()
        {
            _scoringStrategy = new ScoringStrategy();
        }

        [Fact]
        public void CalculatePoints_NoMatches_AllTeamsHaveZeroPoints()
        {
            // Arrange
            var teams = new List<Team>
        {
            new Team { Id = 1 },
            new Team { Id = 2 }
        };
            var matches = new List<Match>();

            // Act
            var result = _scoringStrategy.CalculatePoints(teams, matches);

            // Assert
            Assert.All(result.Values, points => Assert.Equal(0, points));
        }

        [Fact]
        public void CalculatePoints_Team1Wins_Team1Gets3Points()
        {
            // Arrange
            var teams = new List<Team>
        {
            new Team { Id = 1 },
            new Team { Id = 2 }
        };
            var matches = new List<Match>
        {
            new Match { Team1Id = 1, Team2Id = 2, Team1Score = 2, Team2Score = 1 }
        };

            // Act
            var result = _scoringStrategy.CalculatePoints(teams, matches);

            // Assert
            Assert.Equal(3, result[1]);
            Assert.Equal(0, result[2]);
        }

        [Fact]
        public void CalculatePoints_Team2Wins_Team2Gets3Points()
        {
            // Arrange
            var teams = new List<Team>
        {
            new Team { Id = 1 },
            new Team { Id = 2 }
        };
            var matches = new List<Match>
        {
            new Match { Team1Id = 1, Team2Id = 2, Team1Score = 0, Team2Score = 4 }
        };

            // Act
            var result = _scoringStrategy.CalculatePoints(teams, matches);

            // Assert
            Assert.Equal(0, result[1]);
            Assert.Equal(3, result[2]);
        }

        [Fact]
        public void CalculatePoints_Draw_BothTeamsGet1Point()
        {
            // Arrange
            var teams = new List<Team>
        {
            new Team { Id = 1 },
            new Team { Id = 2 }
        };
            var matches = new List<Match>
        {
            new Match { Team1Id = 1, Team2Id = 2, Team1Score = 2, Team2Score = 2 }
        };

            // Act
            var result = _scoringStrategy.CalculatePoints(teams, matches);

            // Assert
            Assert.Equal(1, result[1]);
            Assert.Equal(1, result[2]);
        }

        [Fact]
        public void CalculatePoints_MatchWithUnknownTeam_SkipsMatch()
        {
            // Arrange
            var teams = new List<Team>
        {
            new Team { Id = 1 }
        };
            var matches = new List<Match>
        {
            new Match { Team1Id = 1, Team2Id = 99, Team1Score = 3, Team2Score = 0 }
        };

            // Act
            var result = _scoringStrategy.CalculatePoints(teams, matches);

            // Assert
            Assert.Equal(0, result[1]);
            Assert.False(result.ContainsKey(99));
        }

        [Fact]
        public void CalculatePoints_MultipleMatches_CorrectTotalPoints()
        {
            // Arrange
            var teams = new List<Team>
        {
            new Team { Id = 1 },
            new Team { Id = 2 },
            new Team { Id = 3 }
        };
            var matches = new List<Match>
        {
            new Match { Team1Id = 1, Team2Id = 2, Team1Score = 2, Team2Score = 0 }, 
            new Match { Team1Id = 2, Team2Id = 3, Team1Score = 1, Team2Score = 1 }, 
            new Match { Team1Id = 3, Team2Id = 1, Team1Score = 3, Team2Score = 4 }  
        };

            // Act
            var result = _scoringStrategy.CalculatePoints(teams, matches);

            // Assert
            Assert.Equal(6, result[1]); 
            Assert.Equal(1, result[2]); 
            Assert.Equal(1, result[3]); 
        }
    }
}