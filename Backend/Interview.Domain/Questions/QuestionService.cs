namespace Interview.Domain.Questions
{
    public class QuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<Question> CreateAsync(QuestionCreateRequest request, CancellationToken cancellationToken = default)
        {
            var result = new Question(request.Value);
            await _questionRepository.CreateAsync(result, cancellationToken);
            return result;
        }

        public async Task<Question?> UpdateAsync(QuestionEditRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _questionRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entity == null)
            {
                return null;
            }

            entity.Value = request.Value;
            await _questionRepository.UpdateAsync(entity, cancellationToken);
            return entity;
        }
    }
}
