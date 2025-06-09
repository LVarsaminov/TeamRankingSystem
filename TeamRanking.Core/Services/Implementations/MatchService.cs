using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Models;
using TeamRanking.Core.Repositories.Interfaces;
using TeamRanking.Core.Services.Interfaces;

namespace TeamRanking.Core.Services.Implementations
{
    public class MatchService : IMatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRankingService _rankingService;
        private readonly ITeamRepository _teamRepository;

        public MatchService(IUnitOfWork unitOfWork, IMapper mapper, IRankingService rankingService, ITeamRepository teamRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rankingService = rankingService;
            _teamRepository = teamRepository;
        }

        public async Task<IEnumerable<MatchDto>> GetAllAsync()
        {
            var matches = await _unitOfWork.Matches.GetAllAsync();
            return _mapper.Map<IEnumerable<MatchDto>>(matches);
        }

        public async Task<MatchDto> GetByIdAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            return _mapper.Map<MatchDto>(match);
        }

        public async Task<(string message, MatchDto matchDto)> CreateAsync(CreateMatchDto matchDto)
        {
            var team1 = await _teamRepository.GetByNameAsync(matchDto.Team1Name);
            if (team1 == null)
            {
                return ($"{matchDto.Team1Name} does not exist!", null);
            }
            var team2 = await _teamRepository.GetByNameAsync(matchDto.Team2Name);
            if (team2 == null)
            {
                return ($"{matchDto.Team2Name} does not exist!", null);
            }
            if(team1.Name == team2.Name)
            {
                return ($"{team1.Name} cannot have a match alone", null);
            }
            var match = _mapper.Map<Match>(matchDto);

            team1.PlayedMatchesCount++;
            team2.PlayedMatchesCount++;

            match.Team1Id = team1.Id;
            match.Team2Id = team2.Id;
            match.Team1 = team1;
            match.Team2 = team2;

            await _unitOfWork.Matches.AddAsync(match);

            await _rankingService.UpdateRankingsForCurrentMatchAsync(match);


            return ("", _mapper.Map<MatchDto>(match));
        }

        public async Task<bool> UpdateAsync(int id, UpdateMatchDto matchDto)
        {
            var existingMatch = await _unitOfWork.Matches.GetByIdAsync(id);
            if (existingMatch == null)
                return false;

            existingMatch.Team1Score = matchDto.Team1Score;
            existingMatch.Team2Score = matchDto.Team2Score;
            existingMatch.MatchDate = matchDto.MatchDate;

            _unitOfWork.Matches.Update(existingMatch);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null) return false;

            _unitOfWork.Matches.Delete(match);
            if (match.Team1.PlayedMatchesCount > 0)
            {
                match.Team1.PlayedMatchesCount--;
            }
            if (match.Team2.PlayedMatchesCount > 0)
            {
                match.Team2.PlayedMatchesCount--;
            }
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
