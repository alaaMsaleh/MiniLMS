using MiniLMS.Application.DTOs;
using MiniLMS.Application.DTOs.ChoicesDtos;
using MiniLMS.Application.Interfaces;
using MiniLMS.Domain.Entities;

namespace MiniLMS.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public async Task<QuestionResponseDto> CreateQuestionAsync(QuestionResponceDto dto)
        {
            ValidateChoices(dto.Choices);

            var question = new Question
            {
                Text = dto.Text,
                ImageUrl = dto.ImageUrl,
                Choices = dto.Choices.Select(c => new Choice
                {
                    Text = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            };
            await _questionRepository.AddAsync(question);
            await _questionRepository.SaveChangesAsync();

            return MapToResponseDto(question);

        }



        public async Task<List<QuestionResponseDto>> GetAllQuestionsAsync()
        {
            var questions = await _questionRepository.GetAllAsync();
            return questions.Select(q => MapToResponseDto(q)).ToList();
        }

        public async Task<QuestionResponseDto?> GetQuestionByIdAsync(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            return question == null ? null : MapToResponseDto(question);
        }

        public async Task UpdateQuestionAsync(int id, QuestionResponceDto dto)
        {
            ValidateChoices(dto.Choices);

            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
                throw new KeyNotFoundException($"Question with ID {id} not found.");

            question.Text = dto.Text;
            question.ImageUrl = dto.ImageUrl;


            question.Choices.Clear();
            foreach (var choiceDto in dto.Choices)
            {
                question.Choices.Add(new Choice
                {
                    Text = choiceDto.Text,
                    IsCorrect = choiceDto.IsCorrect
                });
            }

            _questionRepository.Update(question);
            await _questionRepository.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question != null)
            {
                _questionRepository.Delete(question);
                await _questionRepository.SaveChangesAsync();
            }
        }
        private static void ValidateChoices(List<CreateChoiceDto> choices)
        {
            //check nof chices 4 , 6

            if (choices == null || choices.Count < 4 || choices.Count > 6)
            {
                throw new ArgumentException("Question must have between 2 and 6 choices.");
            }


            if (choices.Count(c => c.IsCorrect) != 1)
            {
                throw new ArgumentException("Question must have exactly one correct answer.");
            }
        }


        private static QuestionResponseDto MapToResponseDto(Question question)
        {
            return new QuestionResponseDto
            {
                Id = question.Id,
                Text = question.Text,
                ImageUrl = question.ImageUrl,
                Choices = question.Choices.Select(c => new ChoiceResponseDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            };
        }
    }
}
