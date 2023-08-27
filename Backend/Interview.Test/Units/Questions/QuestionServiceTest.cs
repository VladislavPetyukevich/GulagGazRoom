using Interview.Domain;
using Interview.Domain.Questions;
using Interview.Domain.Tags;
using Moq;

namespace Interview.Test.Units.Questions;

public class QuestionServiceTest
{
    private readonly Mock<IQuestionRepository> _questionRepository;
    private readonly Mock<IQuestionNonArchiveRepository> _questionArchiveRepository;
    private readonly QuestionService _questionService;

    public QuestionServiceTest()
    {
        _questionRepository = new Mock<IQuestionRepository>();

        _questionArchiveRepository = new Mock<IQuestionNonArchiveRepository>();

        var archiveService = new Mock<ArchiveService<Question>>(_questionRepository.Object);
        var questionTag = new Mock<ITagRepository>();

        _questionService = new QuestionService(_questionRepository.Object, _questionArchiveRepository.Object, archiveService.Object, questionTag.Object);
    }

    [Fact(DisplayName = "Searching question by id when question not found")]
    public async Task FindByIdWhenEntityNotFound()
    {
        var questionGuid = Guid.Empty;

        _questionRepository.Setup(repository => repository.FindByIdAsync(questionGuid, default))
            .ReturnsAsync((Question?)null);

        var resultQuestion = await _questionService.FindByIdAsync(questionGuid);

        Assert.True(resultQuestion.IsFailure);

        Assert.NotNull(resultQuestion.Error);
    }

    [Fact(DisplayName = "Searching question by id when question exists")]
    public async Task FindByIdWhenEntityFound()
    {
        var questionGuid = Guid.Empty;

        var questionStub = new Question("value");
        _questionArchiveRepository.Setup(repository => repository.FindByIdAsync(questionGuid, default))
            .ReturnsAsync(questionStub);

        var resultQuestion = await _questionService.FindByIdAsync(questionGuid);

        Assert.True(resultQuestion.IsSuccess);

        var questionItem = resultQuestion.Value;

        Assert.Equal(questionStub.Value, questionItem.Value.Value);
    }
}
