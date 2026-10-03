using Microsoft.AspNetCore.Mvc;
using MiniLMS.Application.DTOs.QuizzesDtos;
using MiniLMS.Application.Interfaces;

namespace MiniLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizeDto dto)
        {
            try
            {
                var quiz = await _quizService.CreateQuizAsync(dto);
                return CreatedAtAction(nameof(GetQuizById), new { id = quiz.Id }, quiz);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuizById(int id)
        {
            var quiz = await _quizService.GetQuizByIdAsync(id);
            if (quiz == null)
                return NotFound(new { message = $"Quiz with ID {id} not found." });

            return Ok(quiz);
        }

        // GET /api/quizzes
        [HttpGet]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();
            return Ok(quizzes);
        }

        // DELETE /api/quizzes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var success = await _quizService.DeleteQuizAsync(id);
            if (!success)
                return NotFound(new { message = $"Quiz with ID {id} not found." });

            return NoContent();
        }

        //    [HttpPost("{id}/submit")]
        //    [Authorize(Roles = "Student")]
        //    public async Task<IActionResult> SubmitQuiz(int id, [FromBody] SubmitQuizDto dto)
        //    {

        //        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        //        var result = await _quizService.SubmitQuizAsync(id, studentId, dto);
        //        return Ok(result);
        //    }

        //}
    }
}