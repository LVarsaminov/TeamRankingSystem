using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;

namespace TeamRanking.Core.Services.Interfaces
{
    public interface IMatchService
    {
        Task<IEnumerable<MatchDto>> GetAllAsync();
        Task<MatchDto> GetByIdAsync(int id);
        Task<(string message, MatchDto matchDto)> CreateAsync(CreateMatchDto createMatchDto);
        Task<bool> UpdateAsync(int id, UpdateMatchDto matchDto);
        Task<bool> DeleteAsync(int id);

    }
}
