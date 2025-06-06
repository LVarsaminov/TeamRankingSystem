using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Models;
using TeamRanking.Core.Repositories.Interfaces;
using TeamRanking.Core.Services.Interfaces;
using static TeamRanking.Core.Constants.GlobalConstants;

namespace TeamRanking.Core.Services.Implementations
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRankingService _rankingService; 

        public TeamService(IUnitOfWork unitOfWork, IMapper mapper, IRankingService rankingService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rankingService = rankingService;
        }

        public async Task<IEnumerable<TeamDto>> GetAllAsync()
        {
            var teams = await _unitOfWork.Teams.GetAllAsync();
            return _mapper.Map<IEnumerable<TeamDto>>(teams);
        }

        public async Task<TeamDto> GetByIdAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            return _mapper.Map<TeamDto>(team);
        }

        public async Task<(string message, TeamDto Team)> CreateAsync(CreateTeamDto teamDto)
        {
            var existingTeam = await _unitOfWork.Teams.GetByNameAsync(teamDto.Name);
            if (existingTeam != null)
            {
                return (teamAlreadyExistsMessage, null);
            }

            var team = _mapper.Map<Team>(teamDto);
            await _unitOfWork.Teams.AddAsync(team);
            await _unitOfWork.SaveChangesAsync();

            await _rankingService.UpdateRankingsAsync();

            return (string.Empty,_mapper.Map<TeamDto>(team));
        }

        public async Task<bool> UpdateAsync(int id, UpdateTeamDto teamDto)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team == null)
                return false;

            team.Name = teamDto.Name;

            _unitOfWork.Teams.Update(team);
            await _unitOfWork.SaveChangesAsync();

            await _rankingService.UpdateRankingsAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team == null) return false;

            _unitOfWork.Teams.Delete(team);
            await _unitOfWork.SaveChangesAsync();

            await _rankingService.UpdateRankingsAsync();

            return true;
        }
    }
}
