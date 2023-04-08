using FluentAssertions;
using Interview.Domain.Questions;
using Interview.Infrastructure.Questions;

namespace Interview.Test.Integrations;

public class QuestionServiceTest
{
    private const string DefaultQuestionValue = "TEST_QUESTION";

    [Fact(DisplayName = "Searching question by id")]
    public async Task FindByIdSuccessful()
    {
        var testSystemClock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(testSystemClock);

        var question = new Question(value: DefaultQuestionValue);

        appDbContext.Questions.Add(question);

        await appDbContext.SaveChangesAsync();

        var questionRepository = new QuestionRepository(appDbContext);
        var questionService = new QuestionService(questionRepository);

        var foundQuestion = await questionService.FindById(question.Id);

        Assert.True(foundQuestion.IsSuccess);

        foundQuestion.Value?.Value.Value.Should().BeEquivalentTo(question.Value);
    }

    [Fact(DisplayName = "Searching question by id when question not found")]
    public async Task FindByIdWhenQuestionNotExists()
    {
        var testSystemClock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(testSystemClock);

        var questionRepository = new QuestionRepository(appDbContext);
        var questionService = new QuestionService(questionRepository);

        var foundQuestion = await questionService.FindById(Guid.NewGuid());

        Assert.True(foundQuestion.IsFailure);

        Assert.NotNull(foundQuestion.Error);
    }
}
