using KTLearningPlatform.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KTLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KtContentController : ControllerBase
    {
        private readonly IKTContentService _contentService;

        public KtContentController(IKTContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet("modules")]
        public async Task<IActionResult> GetModules()
        {
            var modules = await _contentService.GetModulesAsync();
            return Ok(modules);
        }

        [HttpGet("modules/{id}")]
        public async Task<IActionResult> GetModule(int id)
        {
            var module = await _contentService.GetModuleByIdAsync(id);
            if (module == null) return NotFound();
            return Ok(module);
        }

        [HttpGet("sessions/{sessionId}/assets")]
        public async Task<IActionResult> GetAssetsBySession(int sessionId)
        {
            var assets = await _contentService.GetAssetsBySessionAsync(sessionId);
            return Ok(assets);
        }
    }
}
