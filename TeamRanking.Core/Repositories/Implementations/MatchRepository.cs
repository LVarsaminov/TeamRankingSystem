using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TeamRanking.Core.Data;
using TeamRanking.Core.Models;
using TeamRanking.Core.Repositories.Interfaces;

namespace TeamRanking.Core.Repositories.Implementations
{
    public class MatchRepository : IMatchRepository
    {
        private readonly ApplicationDbContext _context;

        public MatchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Match>> GetAllAsync()
        {
            if (_context.Matches.Any())
            {
                return await _context.Matches.Include(m => m.Team1)
                                             .Include(m => m.Team2)
                                             .ToListAsync();
            }
            return null;
        }

        public async Task<Match> GetByIdAsync(int id)
        {
            return await _context.Matches.Include(m => m.Team1)
                                         .Include(m => m.Team2)
                                         .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Match match)
        {
            await _context.Matches.AddAsync(match);
            await _context.SaveChangesAsync();
        }

        public void Update(Match match)
        {
            _context.Matches.Update(match);
        }

        public void Delete(Match match)
        {
            _context.Matches.Remove(match);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Matches.AnyAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Match>> FindAsync(Expression<Func<Match, bool>> predicate)
        {
            return await _context.Matches.Where(predicate).ToListAsync();
        }
    }
}
