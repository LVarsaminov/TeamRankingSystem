using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Models;

namespace TeamRanking.Core.Repositories.Interfaces
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllAsync();
        Task<Team> GetByIdAsync(int id);
        Task AddAsync(Team team);
        void Update(Team team);
        void Delete(Team team);
        Task<bool> ExistsAsync(int id);
        Task<Team> GetByNameAsync(string name);
    }
}
