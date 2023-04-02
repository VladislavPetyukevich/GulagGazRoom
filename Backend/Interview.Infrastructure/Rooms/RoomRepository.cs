using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service.Records.Response;
using Interview.Domain.Rooms.Service.Records.Response.Detail;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Interview.Infrastructure.Rooms;

public class RoomRepository : EfRepository<Room>, IRoomRepository
{
    public RoomRepository(AppDbContext db)
        : base(db)
    {
    }

    public async Task<Analytics?> GetAnalyticsAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var analytics = await Set.AsNoTracking()
            .Include(e => e.Questions)
                .ThenInclude(e => e.Question)
            .Include(e => e.Participants)
            .Where(e => e.Id == roomId)
            .Select(e => new Analytics
            {
                Questions = e.Questions.Select(q => new Analytics.AnalyticsQuestion
                {
                    Id = q.Question.Id,
                    Status = q.State.Name,
                    Value = q.Question.Value,
                }).ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (analytics == null)
        {
            return null;
        }

        var reactions = await Db.RoomQuestionReactions.AsNoTracking()
            .Include(e => e.Sender)
            .Include(e => e.Reaction)
            .Include(e => e.RoomQuestion)
                .ThenInclude(e => e.Question)
            .Where(e => e.RoomQuestion.Room.Id == roomId)
            .ToListAsync(cancellationToken);

        var users = reactions.Select(e => e.Sender.Id).Distinct();
        var participants = await Db.RoomParticipants.AsNoTracking()
            .Include(e => e.Room)
            .Include(e => e.User)
            .Where(e => e.Room.Id == roomId && users.Contains(e.User.Id))
            .ToDictionaryAsync(e => e.User.Id, cancellationToken);

        analytics.Reactions = reactions.Select(e => e.Reaction)
            .GroupBy(e => (e.Id, e.Type))
            .Select(e => new Analytics.AnalyticsReactionSummary
            {
                Id = e.Key.Id,
                Count = e.Count(),
                Type = e.Key.Type.Name,
            })
            .ToList();

        var questionReaction = reactions.ToLookup(e => e.RoomQuestion.Question.Id);
        foreach (var analyticsQuestion in analytics.Questions!)
        {
            if (!questionReaction[analyticsQuestion.Id].Any())
            {
                continue;
            }

            analyticsQuestion.Users = questionReaction[analyticsQuestion.Id]
                .GroupBy(e => e.Sender.Id).Select(e =>
                {
                    var sender = e.First().Sender;
                    participants.TryGetValue(sender.Id, out var participant);
                    return new Analytics.AnalyticsUser
                    {
                        Id = sender.Id,
                        Avatar = string.Empty,
                        Nickname = sender.Nickname,
                        ParticipantType = participant?.Type.Name ?? string.Empty,
                        Reactions = e.Select(roomQuestionReaction => new Analytics.AnalyticsReaction
                        {
                            Id = roomQuestionReaction.Reaction.Id,
                            Type = roomQuestionReaction.Reaction.Type.Name,
                            CreatedAt = roomQuestionReaction.CreateDate,
                        }).ToList(),
                        ReactionsSummary = e.GroupBy(roomQuestionReaction => (roomQuestionReaction.Reaction.Id, roomQuestionReaction.Reaction.Type))
                            .Select(roomQuestionReactions => new Analytics.AnalyticsReactionSummary
                            {
                                Id = roomQuestionReactions.Key.Id,
                                Count = roomQuestionReactions.Count(),
                                Type = roomQuestionReactions.Key.Type.Name,
                            }).ToList(),
                    };
                }).ToList();
        }

        return analytics;
    }

    public Task<bool> HasUserAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        return Set
            .Include(room => room.Participants)
            .AnyAsync(
                room => room.Id == roomId && room.Participants.Any(participant => participant.User.Id == userId),
                cancellationToken);
    }

    public Task<IPagedList<RoomDetail>> GetDetailedPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return GetRoomDetailQueryable()
            .OrderBy(e => e.Id)
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<RoomDetail?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return GetRoomDetailQueryable()
            .FirstOrDefaultAsync(room => room.Id == roomId, cancellationToken: cancellationToken);
    }

    protected override IQueryable<Room> ApplyIncludes(DbSet<Room> set)
        => Set.Include(e => e.Participants)
            .Include(e => e.Questions);

    private IQueryable<RoomDetail> GetRoomDetailQueryable()
    {
        return Set
            .Include(e => e.Participants)
            .Include(e => e.Questions)
            .Select(e => new RoomDetail
            {
                Id = e.Id,
                Name = e.Name,
                TwitchChannel = e.TwitchChannel,
                Questions = e.Questions.Select(question => question.Question)
                    .Select(question => new RoomQuestionDetail { Id = question.Id, Value = question.Value, })
                    .ToList(),
                Users = e.Participants.Select(participant =>
                        new RoomUserDetail { Id = participant.User.Id, Nickname = participant.User.Nickname, })
                    .ToList(),
            });
    }
}
