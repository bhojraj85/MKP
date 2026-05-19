using KTLearningPlatform.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KTLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminDashboardService _adminService;

        public AdminController(IAdminDashboardService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardMetrics()
        {
            var metrics = await _adminService.GetDashboardMetricsAsync();
            return Ok(metrics);
        }

        [HttpGet("compliance")]
        public async Task<IActionResult> GetComplianceReport()
        {
            var report = await _adminService.GetComplianceReportAsync();
            return Ok(report);
        }
    }
}
