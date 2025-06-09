using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TeamRanking.Core.Models;

namespace TeamRanking.Core.Repositories.Interfaces
{
    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetAllAsync();
        Task<Match> GetByIdAsync(int id);
        Task AddAsync(Match match);
        void Update(Match match);
        void Delete(Match match);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Match>> FindAsync(Expression<Func<Match, bool>> predicate);
    }
}
