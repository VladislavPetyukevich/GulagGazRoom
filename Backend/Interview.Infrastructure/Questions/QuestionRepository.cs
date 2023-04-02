using Interview.Domain.Questions;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Questions;

public class QuestionRepository : EfRepository<Question>, IQuestionRepository
{
    public QuestionRepository(AppDbContext db)
        : base(db)
    {
    }

    public Task DeletePermanentlyAsync(Question entity, CancellationToken cancellationToken = default)
    {
        var questions = Db.Questions.Where(question => question.Id == entity.Id);

        var roomQuestions = Db.RoomQuestions
            .Include(roomQuestion => roomQuestion.Question)
            .Where(roomQuestion => roomQuestion.Question.Id == entity.Id);

        var roomQuestionReactions = Db.RoomQuestionReactions
            .Include(roomQuestionReaction => roomQuestionReaction.RoomQuestion)
            .Include(roomQuestionReaction => roomQuestionReaction.RoomQuestion.Question)
            .Where(roomQuestionReaction => roomQuestionReaction.RoomQuestion.Question.Id == entity.Id);

        Db.RoomQuestionReactions.RemoveRange(roomQuestionReactions);
        Db.RoomQuestions.RemoveRange(roomQuestions);
        Db.Questions.RemoveRange(questions);

        return Db.SaveChangesAsync(cancellationToken);
    }

    protected override IQueryable<Question> ApplyIncludes(DbSet<Question> set) => set;
}
