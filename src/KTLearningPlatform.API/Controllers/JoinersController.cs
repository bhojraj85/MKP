using KTLearningPlatform.Core.DTOs;
using KTLearningPlatform.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KTLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JoinersController : ControllerBase
    {
        private readonly IJoinerService _joinerService;

        public JoinersController(IJoinerService joinerService)
        {
            _joinerService = joinerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var joiners = await _joinerService.GetJoinersAsync();
            return Ok(joiners);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var joiner = await _joinerService.GetJoinerByIdAsync(id);
            if (joiner == null) return NotFound();
            return Ok(joiner);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJoinerDto request)
        {
            var joiner = await _joinerService.CreateJoinerAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = joiner.JoinerId }, joiner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJoinerDto request)
        {
            var updated = await _joinerService.UpdateJoinerAsync(id, request);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _joinerService.DeleteJoinerAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("{id}/dashboard")]
        public async Task<IActionResult> GetDashboard(int id)
        {
            var dashboard = await _joinerService.GetJoinerDashboardAsync(id);
            return Ok(dashboard);
        }
    }
}
