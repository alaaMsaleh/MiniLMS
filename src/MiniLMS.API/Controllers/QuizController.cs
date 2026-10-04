using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniLMS.Application.DTOs.QuizzesDtos;
using MiniLMS.Application.Interfaces;

namespace MiniLMS.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService) => _quizService = quizService;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuizeDto dto)
        {
            var quiz = await _quizService.CreateQuizAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _quizService.GetAllQuizzesAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quiz = await _quizService.GetQuizByIdAsync(id);
            return quiz is null
                ? NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Not found",
                    Detail = $"Quiz with ID {id} not found."
                })
                : Ok(quiz);
        }

        [HttpPost("{id:int}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            await _quizService.SetPublishedAsync(id, true);
            return NoContent();
        }

        [HttpPost("{id:int}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            await _quizService.SetPublishedAsync(id, false);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _quizService.DeleteQuizAsync(id);
            return NoContent();
        }
    }
}