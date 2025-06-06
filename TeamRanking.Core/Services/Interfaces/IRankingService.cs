using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Models;

public interface IRankingService
{
    Task<IEnumerable<TeamDto>> GetRankingsAsync();

    Task UpdateRankingsAsync();

    Task UpdateRankingsForCurrentMatchAsync(Match match);
}