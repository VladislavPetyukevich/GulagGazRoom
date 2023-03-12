using Interview.Domain.Questions;
using Interview.Infrastructure.Questions;
using Moq;

namespace Interview.Test.Units.Questions
{
    public class QuestionServiceTest
    {
        private readonly Mock<IQuestionRepository> _questionRepository;
        private readonly QuestionService _questionService;

        public QuestionServiceTest()
        {
            _questionRepository = new Mock<IQuestionRepository>();

            _questionService = new QuestionService(_questionRepository.Object);
        }

        [Fact(DisplayName = "Searching question by id when question not found")]
        public async Task FindByIdWhenEntityNotFound()
        {
            var questionGuid = Guid.Empty;

            _questionRepository.Setup(repository => repository.FindByIdAsync(questionGuid, default))
                .ReturnsAsync((Question?)null);
            
            var resultQuestion = await _questionService.FindById(questionGuid);
            
            Assert.True(resultQuestion.IsFailure);
            
            Assert.NotNull(resultQuestion.Error);
        }
        
        [Fact(DisplayName = "Searching question by id when question exists")]
        public async Task FindByIdWhenEntityFound()
        {
            var questionGuid = Guid.Empty;

            var questionStub = new Question("value");
            _questionRepository.Setup(repository => repository.FindByIdAsync(questionGuid, default))
                .ReturnsAsync(questionStub);
            
            var resultQuestion = await _questionService.FindById(questionGuid);
            
            Assert.True(resultQuestion.IsSuccess);

            var questionItem = resultQuestion.Value;
            
            Assert.Equal(questionStub.Value, questionItem.Value);
        }
    }
}
