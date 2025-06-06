using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Services.Interfaces;

namespace TeamRanking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly IRankingService _rankingService;

        public MatchesController(IMatchService matchService, IRankingService rankingService)
        {
            _matchService = matchService;
            _rankingService = rankingService;
        }

        // GET: api/matches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetAll()
        {
            var matches = await _matchService.GetAllAsync();
            return Ok(matches);
        }

        // GET: api/matches/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetById(int id)
        {
            var match = await _matchService.GetByIdAsync(id);

            if (match == null)
                return NotFound();

            return Ok(match);
        }

        // POST: api/matches
        [HttpPost]
        public async Task<ActionResult<(string message, MatchDto matchDto)>> Create(CreateMatchDto createMatchDto)
        {
            var createdMatch = await _matchService.CreateAsync(createMatchDto);

            // Update team rankings after match is created
            await _rankingService.UpdateRankingsAsync();

            if (!string.IsNullOrEmpty(createdMatch.message))
            {
                return Ok(createdMatch.message);
            }

            return Ok(createdMatch.matchDto);
        }

        // PUT: api/matches/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateMatchDto updateMatchDto)
        {
            var result = await _matchService.UpdateAsync(id, updateMatchDto);

            if (!result)
                return NotFound();

            // Update team rankings after match is updated
            await _rankingService.UpdateRankingsAsync();

            return NoContent();
        }

        // DELETE: api/matches/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _matchService.DeleteAsync(id);

            if (!result)
                return NotFound();

            // Update team rankings after match is deleted
            await _rankingService.UpdateRankingsAsync();

            return NoContent();
        }

        // GET: api/matches/rankings
        [HttpGet("rankingList")]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetRankings()
        {
            var rankings = await _rankingService.GetRankingsAsync();
            return Ok(rankings);
        }
    }
}
