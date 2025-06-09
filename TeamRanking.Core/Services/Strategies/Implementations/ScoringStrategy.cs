using System.Collections.Generic;
using System.Linq;
using TeamRanking.Core.Models;

namespace TeamRanking.Core.Services.Strategies
{
    public class ScoringStrategy : IScoringStrategy
    {
        public Dictionary<int, int> CalculatePoints(List<Team> teams, List<Match> matches)
        {
            var points = teams.ToDictionary(t => t.Id, t => 0);

            foreach (var match in matches)
            {
                if (!points.ContainsKey(match.Team1Id) || !points.ContainsKey(match.Team2Id))
                    continue;

                if (match.Team1Score > match.Team2Score)
                {
                    points[match.Team1Id] += 3;
                }
                else if (match.Team1Score < match.Team2Score)
                {
                    points[match.Team2Id] += 3;
                }
                else
                {
                    points[match.Team1Id] += 1;
                    points[match.Team2Id] += 1;
                }
            }

            return points;
        }
    }
}