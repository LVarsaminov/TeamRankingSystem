using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamRanking.Core.Data;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Models;
using TeamRanking.Core.Repositories.Interfaces;

namespace TeamRanking.Core.Repositories.Implementations
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<Team> GetByIdAsync(int id)
        {
            return await _context.Teams.FindAsync(id);
        }

        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
        }

        public void Update(Team team)
        {
            _context.Teams.Update(team);

        }

        public void Delete(Team team)
        {
            _context.Teams.Remove(team);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Teams.AnyAsync(t => t.Id == id);
        }
        public async Task<Team> GetByNameAsync(string name)
        {
            return await _context.Teams
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }
    }
}
