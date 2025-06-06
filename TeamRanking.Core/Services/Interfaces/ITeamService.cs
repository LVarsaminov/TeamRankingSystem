using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;

namespace TeamRanking.Core.Services.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDto>> GetAllAsync();
        Task<TeamDto> GetByIdAsync(int id);
        Task<(string message, TeamDto Team)> CreateAsync(CreateTeamDto teamDto);
        Task<bool> UpdateAsync(int id, UpdateTeamDto teamDto);
        Task<bool> DeleteAsync(int id);
    }
}
