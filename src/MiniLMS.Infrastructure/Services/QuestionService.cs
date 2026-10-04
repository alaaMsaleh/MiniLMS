using Microsoft.Extensions.Logging;
using MiniLMS.Application.DTOs;
using MiniLMS.Application.DTOs.ChoicesDtos;
using MiniLMS.Application.DTOs.QuestionDtos;
using MiniLMS.Application.Interfaces;
using MiniLMS.Domain.Entities;

namespace MiniLMS.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repository;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(IQuestionRepository repository, ILogger<QuestionService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<QuestionResponseDto> CreateQuestionAsync(SaveQuestionDto dto)
        {
            Validate(dto);

            var question = new Question
            {
                Text = dto.Text.Trim(),
                ImageUrl = NormalizeUrl(dto.ImageUrl),
                Choices = dto.Choices.Select(ToChoice).ToList()
            };

            await _repository.AddAsync(question);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Question {QuestionId} created with {ChoiceCount} choices",
                question.Id, question.Choices.Count);

            return MapToDto(question);
        }

        public async Task<List<QuestionResponseDto>> GetAllQuestionsAsync()
        {
            var questions = await _repository.GetAllAsync();
            return questions.Select(MapToDto).ToList();
        }

        public async Task<QuestionResponseDto?> GetQuestionByIdAsync(int id)
        {
            var question = await _repository.GetByIdAsync(id);
            return question is null ? null : MapToDto(question);
        }

        public async Task<QuestionResponseDto> UpdateQuestionAsync(int id, SaveQuestionDto dto)
        {
            Validate(dto);

            Question? updated = null;

            await _repository.ExecuteInTransactionAsync(async () =>
            {
                var question = await _repository.GetByIdAsync(id)
                    ?? throw new KeyNotFoundException($"Question with ID {id} not found.");

                question.Text = dto.Text.Trim();
                question.ImageUrl = NormalizeUrl(dto.ImageUrl);
                question.UpdatedAt = DateTime.UtcNow;

                // 1 Soft Delete
                foreach (var oldChoice in question.Choices.ToList())
                    oldChoice.IsDeleted = true;
                await _repository.SaveChangesAsync();

                // 2 save  unique index)
                foreach (var choice in dto.Choices)
                    question.Choices.Add(ToChoice(choice));
                await _repository.SaveChangesAsync();

                updated = question;
            });

            _logger.LogInformation("Question {QuestionId} updated", id);
            return MapToDto(updated!);
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var question = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Question with ID {id} not found.");

            // Soft Delete:
            question.IsDeleted = true;
            question.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Question {QuestionId} soft-deleted", id);
        }

        // ---------------- helpers ----------------

        private static void Validate(SaveQuestionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Text))
                throw new ArgumentException("Question text is required.");

            if (dto.Choices is null || dto.Choices.Count < 2 || dto.Choices.Count > 6)
                throw new ArgumentException("A question must have between 2 and 6 choices.");

            if (dto.Choices.Any(c => string.IsNullOrWhiteSpace(c.Text)))
                throw new ArgumentException("Choice text cannot be empty.");

            if (dto.Choices.Count(c => c.IsCorrect) != 1)
                throw new ArgumentException("A question must have exactly one correct choice.");

            var distinctTexts = dto.Choices.Select(c => c.Text.Trim().ToLowerInvariant()).Distinct().Count();
            if (distinctTexts != dto.Choices.Count)
                throw new ArgumentException("Choices must be unique.");

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                var validUrl = Uri.TryCreate(dto.ImageUrl, UriKind.Absolute, out var uri)
                               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
                if (!validUrl)
                    throw new ArgumentException("Image URL must be a valid http or https URL.");
            }
        }

        private static string? NormalizeUrl(string? url) =>
            string.IsNullOrWhiteSpace(url) ? null : url.Trim();

        private static Choice ToChoice(CreateChoiceDto c) =>
            new() { Text = c.Text.Trim(), IsCorrect = c.IsCorrect };

        private static QuestionResponseDto MapToDto(Question question) => new()
        {
            Id = question.Id,
            Text = question.Text,
            ImageUrl = question.ImageUrl,
            Choices = question.Choices
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Id)
                .Select(c => new ChoiceResponseDto { Id = c.Id, Text = c.Text, IsCorrect = c.IsCorrect })
                .ToList()
        };
    }
}
