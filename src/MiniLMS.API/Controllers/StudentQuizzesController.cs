using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniLMS.Application.DTOs.QuizzesDtos;
using MiniLMS.Application.Interfaces;
using System.Security.Claims;

namespace MiniLMS.API.Controllers
{
    [Route("api/student")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentQuizzesController : ControllerBase
    {
        private readonly IStudentQuizService _service;

        public StudentQuizzesController(IStudentQuizService service) => _service = service;


        private int StudentId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("quizzes")]
        public async Task<IActionResult> GetAvailable() =>
            Ok(await _service.GetAvailableQuizzesAsync());

        [HttpGet("quizzes/{quizId:int}")]
        public async Task<IActionResult> GetQuiz(int quizId) =>
            Ok(await _service.GetQuizAsync(quizId));

        [HttpPost("quizzes/{quizId:int}/submit")]
        public async Task<IActionResult> Submit(int quizId, [FromBody] SubmitQuizDto dto) =>
            Ok(await _service.SubmitQuizAsync(quizId, StudentId, dto));


        [HttpGet("results")]
        public async Task<IActionResult> GetMyResults() =>
    Ok(await _service.GetMyResultsAsync(StudentId));

        [HttpGet("results/{submissionId:int}")]
        public async Task<IActionResult> GetMyResult(int submissionId) =>
            Ok(await _service.GetMyResultAsync(submissionId, StudentId));
    }
}