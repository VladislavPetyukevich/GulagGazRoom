using FluentAssertions;
using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Interview.Infrastructure.Database;
using Interview.Infrastructure.Questions;
using Interview.Infrastructure.Reactions;
using Interview.Infrastructure.RoomQuestionReactions;
using Interview.Infrastructure.RoomQuestions;
using Interview.Infrastructure.Rooms;
using Interview.Infrastructure.Users;

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
        var questionArchiveRepository = new QuestionArchiveRepository(appDbContext);
        var questionService = new QuestionService(questionRepository, questionArchiveRepository);

        var foundQuestion = await questionService.FindByIdAsync(question.Id);

        Assert.True(foundQuestion.IsSuccess);

        foundQuestion.Value?.Value.Value.Should().BeEquivalentTo(question.Value);
    }

    [Fact(DisplayName = "Searching question by id when question not found")]
    public async Task FindByIdWhenQuestionNotExists()
    {
        var testSystemClock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(testSystemClock);

        var questionRepository = new QuestionRepository(appDbContext);
        var questionArchiveRepository = new QuestionArchiveRepository(appDbContext);
        var questionService = new QuestionService(questionRepository, questionArchiveRepository);

        var foundQuestion = await questionService.FindByIdAsync(Guid.NewGuid());

        Assert.True(foundQuestion.IsFailure);

        Assert.NotNull(foundQuestion.Error);
    }

    [Fact(DisplayName = "Permanent deleting the question")]
    public async Task DeletePermanentQuestion()
    {
        await using var appDbContext = new TestAppDbContextFactory().Create(new TestSystemClock());

        var transaction = await appDbContext.Database.BeginTransactionAsync();

        var user = new User("nickname", "twitchChannel");
        appDbContext.Users.Add(user);
        appDbContext.SaveChanges();

        var room = new Room("room#1", "twitch");
        appDbContext.Rooms.Add(room);
        appDbContext.SaveChanges();

        var question = new Question("question#1");
        appDbContext.Questions.Add(question);
        appDbContext.SaveChanges();

        var reaction = new Reaction { Type = ReactionType.Like };
        appDbContext.Reactions.Add(reaction);
        appDbContext.SaveChanges();

        var roomQuestion = new RoomQuestion() { Room = room, Question = question, State = RoomQuestionState.Active };
        appDbContext.RoomQuestions.Add(roomQuestion);
        appDbContext.SaveChanges();

        var roomQuestionReaction = new RoomQuestionReaction
        {
            Reaction = reaction, RoomQuestion = roomQuestion, Sender = user
        };
        appDbContext.RoomQuestionReactions.Add(roomQuestionReaction);
        appDbContext.SaveChanges();
        
        await transaction.CommitAsync();
        
        var questionRepository = new QuestionRepository(appDbContext);
        var questionArchiveRepository = new QuestionArchiveRepository(appDbContext);
        var questionService = new QuestionService(questionRepository, questionArchiveRepository);
        
        var result = await questionService.DeletePermanentlyAsync(question.Id);
        
        Assert.True(result.IsSuccess);
    }
}
