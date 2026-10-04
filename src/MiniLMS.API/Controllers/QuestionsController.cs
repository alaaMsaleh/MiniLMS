using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniLMS.Application.DTOs;
using MiniLMS.Application.DTOs.QuestionDtos;
using MiniLMS.Application.Interfaces;


namespace MiniLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(QuestionResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] SaveQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _questionService.CreateQuestionAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            return question is null
                ? NotFound(new ProblemDetails { Status = 404, Title = "Not found", Detail = $"Question with ID {id} not found." })
                : Ok(question);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _questionService.GetAllQuestionsAsync());

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveQuestionDto dto) =>
            Ok(await _questionService.UpdateQuestionAsync(id, dto));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _questionService.DeleteQuestionAsync(id);
            return NoContent();
        }
    }
}
