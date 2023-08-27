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

    protected override IQueryable<Question> ApplyIncludes(DbSet<Question> set)
    {
        return set
            .Include(e => e.Tags)
            .ThenInclude(e => e.Tag);
    }

    public async Task DeletePermanentlyAsync(Question entity, CancellationToken cancellationToken = default)
    {
        var transaction = await Db.Database.BeginTransactionAsync(cancellationToken);

        await Db.RoomQuestionReactions
            .Where(roomQuestionReaction => roomQuestionReaction.RoomQuestion.Question.Id == entity.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await Db.RoomQuestions
            .Where(roomQuestion => roomQuestion.Question.Id == entity.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await Db.Questions
            .Where(question => question.Id == entity.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}
