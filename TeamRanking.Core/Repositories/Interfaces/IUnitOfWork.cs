using System.Threading.Tasks;

namespace TeamRanking.Core.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        ITeamRepository Teams { get; }
        IMatchRepository Matches { get; }

        Task<int> SaveChangesAsync();
    }
}
