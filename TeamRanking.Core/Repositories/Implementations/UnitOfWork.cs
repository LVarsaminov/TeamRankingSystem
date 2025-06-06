using System.Threading.Tasks;
using TeamRanking.Core.Data;
using TeamRanking.Core.Repositories.Interfaces;

namespace TeamRanking.Core.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Teams = new TeamRepository(context);
            Matches = new MatchRepository(context);
        }

        public ITeamRepository Teams { get; }
        public IMatchRepository Matches { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
