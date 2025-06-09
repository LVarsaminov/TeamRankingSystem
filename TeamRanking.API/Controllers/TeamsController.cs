using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Core.DTOs;
using TeamRanking.Core.Services.Interfaces;
using static TeamRanking.Core.Constants.GlobalConstants;

namespace TeamRanking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        // GET: api/teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetAll()
        {
            var teams = await _teamService.GetAllAsync();
            return Ok(teams);
        }

        // GET: api/teams/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TeamDto>> GetById(int id)
        {
            var team = await _teamService.GetByIdAsync(id);

            if (team == null)
                return NotFound(doesNotExistMessage);

            return Ok(team);
        }

        // POST: api/teams
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<(string message, TeamDto teamDto)>> Create(CreateTeamDto createTeamDto)
        {
            var createdTeam = await _teamService.CreateAsync(createTeamDto);
            if(!string.IsNullOrEmpty(createdTeam.message))
            {
                return NotFound(createdTeam.message);
            }
            return Ok(createdTeam.Team);
        }

        // PUT: api/teams/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, UpdateTeamDto updateTeamDto)
        {
            var result = await _teamService.UpdateAsync(id, updateTeamDto);

            if (!result)
                return NotFound(doesNotExistMessage);

            return Ok(updatedMessage);
        }

        // DELETE: api/teams/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _teamService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok(deleteMatchSuccessfuly);
        }
    }
}
