using Microsoft.AspNetCore.Mvc;
using MiniLMS.Application.DTOs;
using MiniLMS.Application.Interfaces;


namespace MiniLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        //CRUD
        //Create question 
        [HttpPost("Add_Question")] //POST /api/questions

        public async Task<ActionResult> CreateQuection([FromBody] QuestionResponceDto dto)
        {
            try
            {
                var result = await _questionService.CreateQuestionAsync(dto);

                return CreatedAtAction(nameof(GetQuestionById), new { id = result.Id }, result);

            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
                return NotFound(new { message = $"Question with ID {id} not found." });

            return Ok(question);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuestions()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionResponceDto dto)
        {
            try
            {
                await _questionService.UpdateQuestionAsync(id, dto);
                return NoContent();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var existingQuestion = await _questionService.GetQuestionByIdAsync(id);
            if (existingQuestion == null)
                return NotFound(new { message = $"Question with ID {id} not found." });

            await _questionService.DeleteQuestionAsync(id);
            return NoContent(); // 204 No Content
        }


    }
}
