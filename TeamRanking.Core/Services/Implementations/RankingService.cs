using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Models;
using TeamRanking.Core.Repositories.Interfaces;
using TeamRanking.Core.Services.Interfaces;
using TeamRanking.Core.Services.Strategies;
using AutoMapper;

namespace TeamRanking.Core.Services.Implementations
{
    public class RankingService : IRankingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IScoringStrategy _scoringStrategy;

        public RankingService(IUnitOfWork unitOfWork, IMapper mapper, IScoringStrategy scoringStrategy)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _scoringStrategy = scoringStrategy;
        }

        public async Task<IEnumerable<TeamDto>> GetRankingsAsync()
        {
            var teams = await _unitOfWork.Teams.GetAllAsync() ?? new List<Team>();
            var matches = await _unitOfWork.Matches.GetAllAsync() ?? new List<Match>();

            var teamPoints = _scoringStrategy.CalculatePoints(teams.ToList(), matches.ToList())
                                                                ?? new Dictionary<int, int>();

            var teamDtos = _mapper.Map<IEnumerable<TeamDto>>(teams);

            foreach (var teamDto in teamDtos)
            {
                teamDto.Points = teamPoints.TryGetValue(teamDto.Id, out var points) ? points : 0;
            }

            return teamDtos.OrderByDescending(t => t.Points);
        }

        public async Task UpdateRankingsAsync()
        {
            var teams = await _unitOfWork.Teams.GetAllAsync();
            var matches = await _unitOfWork.Matches.GetAllAsync() ?? new List<Match>();

            var teamPoints = _scoringStrategy.CalculatePoints(teams.ToList(), matches.ToList());


            foreach (var team in teams)
            {
                if (teamPoints.TryGetValue(team.Id, out var points))
                    team.Points = points;
                else
                    team.Points = 0;
                _unitOfWork.Teams.Update(team);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateRankingsForCurrentMatchAsync(Match match)
        {
            var teams = await _unitOfWork.Teams.GetAllAsync(); 
            var matches = await _unitOfWork.Matches.GetAllAsync();

            if (!matches.Any())
            {
                return;
            }

            var teamPoints = _scoringStrategy.CalculatePoints(teams.ToList(), matches.ToList());

            foreach (var teamId in new[] { match.Team1Id, match.Team2Id })
            {
                var team = teams.FirstOrDefault(t => t.Id == teamId);
                if (team != null && teamPoints.TryGetValue(teamId, out var points))
                {
                    team.Points = points;
                    _unitOfWork.Teams.Update(team);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
