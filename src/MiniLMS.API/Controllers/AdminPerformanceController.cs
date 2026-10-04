using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniLMS.Application.Interfaces;

namespace MiniLMS.API.Controllers
{
    [Route("api/admin/performance")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPerformanceController : ControllerBase
    {
        private readonly IPerformanceService _service;

        public AdminPerformanceController(IPerformanceService service) => _service = service;

        // GET /api/admin/performance?quizId=1&studentId=2  
        [HttpGet]
        public async Task<IActionResult> GetAttempts([FromQuery] int? quizId, [FromQuery] int? studentId) =>
            Ok(await _service.GetAttemptsAsync(quizId, studentId));

        // GET /api/admin/performance/students
        [HttpGet("students")]
        public async Task<IActionResult> GetStudentsOverview() =>
            Ok(await _service.GetStudentsOverviewAsync());

        // GET /api/admin/performance/submissions/5
        [HttpGet("submissions/{submissionId:int}")]
        public async Task<IActionResult> GetSubmission(int submissionId) =>
            Ok(await _service.GetSubmissionAsync(submissionId));
    }
}