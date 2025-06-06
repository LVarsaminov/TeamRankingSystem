using System.Collections.Generic;
using TeamRanking.Core.Models;

namespace TeamRanking.Core.Services.Strategies
{
    public interface IScoringStrategy
    {
        Dictionary<int, int> CalculatePoints(List<Team> teams, List<Match> matches);
    }
}