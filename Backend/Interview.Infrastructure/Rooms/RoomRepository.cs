using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service.Records.Request;
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

    public async Task<Analytics?> GetAnalyticsAsync(RoomAnalyticsRequest request, CancellationToken cancellationToken = default)
    {
        var analytics = await GetAnalyticsCoreAsync(request.RoomId, cancellationToken);
        if (analytics == null)
        {
            return null;
        }

        var reactions = await GetRoomQuestionReactionAsync(request.RoomId, cancellationToken);

        analytics.Reactions = GetReactions(reactions);

        var questionReaction = reactions.ToLookup(e => e.RoomQuestion.Question.Id);
        foreach (var analyticsQuestion in analytics.Questions!)
        {
            if (!questionReaction[analyticsQuestion.Id].Any())
            {
                continue;
            }

            analyticsQuestion.Users = await GetUsersAsync(questionReaction[analyticsQuestion.Id]);
        }

        return analytics;

        Task<List<RoomQuestionReaction>> GetRoomQuestionReactionAsync(Guid roomId, CancellationToken ct)
        {
            return Db.RoomQuestionReactions.AsNoTracking()
                .Include(e => e.Sender)
                .Include(e => e.Reaction)
                .Include(e => e.RoomQuestion)
                    .ThenInclude(e => e.Question)
                .Where(e => e.RoomQuestion.Room.Id == roomId)
                .ToListAsync(ct);
        }

        Task<Analytics?> GetAnalyticsCoreAsync(Guid roomId, CancellationToken ct)
        {
            return Set.AsNoTracking()
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
                .FirstOrDefaultAsync(ct);
        }

        List<Analytics.AnalyticsReactionSummary> GetReactions(List<RoomQuestionReaction> roomQuestionReactions)
        {
            return roomQuestionReactions
                .Select(e => e.Reaction)
                .GroupBy(e => (e.Id, e.Type))
                .Select(e => new Analytics.AnalyticsReactionSummary
                {
                    Id = e.Key.Id,
                    Count = e.Count(),
                    Type = e.Key.Type.Name,
                })
                .ToList();
        }

        async Task<List<Analytics.AnalyticsUser>> GetUsersAsync(IEnumerable<RoomQuestionReaction> roomReactions)
        {
            var users = reactions.Select(e => e.Sender.Id).Distinct();

            var participants = await Db.RoomParticipants.AsNoTracking()
                .Include(e => e.Room)
                .Include(e => e.User)
                .Where(e => e.Room.Id == request.RoomId &&
                            users.Contains(e.User.Id) &&
                            (request.SpecificUserIds.Count == 0 || request.SpecificUserIds.Contains(e.User.Id)))
                .ToDictionaryAsync(e => e.User.Id, cancellationToken);

            return roomReactions
                .Where(e => request.SpecificUserIds.Count == 0 || request.SpecificUserIds.Contains(e.Sender.Id))
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
                        Reactions = ToAnalyticsReaction(e),
                        ReactionsSummary = ToAnalyticsReactionSummary(e),
                    };
                }).ToList();
        }

        static List<Analytics.AnalyticsReaction> ToAnalyticsReaction(IGrouping<Guid, RoomQuestionReaction> e)
        {
            return e.Select(roomQuestionReaction => new Analytics.AnalyticsReaction
            {
                Id = roomQuestionReaction.Reaction.Id,
                Type = roomQuestionReaction.Reaction.Type.Name,
                CreatedAt = roomQuestionReaction.CreateDate,
            }).ToList();
        }

        static List<Analytics.AnalyticsReactionSummary> ToAnalyticsReactionSummary(IGrouping<Guid, RoomQuestionReaction> e)
        {
            return e.GroupBy(roomQuestionReaction => (roomQuestionReaction.Reaction.Id, roomQuestionReaction.Reaction.Type))
                .Select(roomQuestionReactions => new Analytics.AnalyticsReactionSummary
                {
                    Id = roomQuestionReactions.Key.Id,
                    Count = roomQuestionReactions.Count(),
                    Type = roomQuestionReactions.Key.Type.Name,
                }).ToList();
        }
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
                    .Select(question => new RoomQuestionDetail { Id = question!.Id, Value = question.Value, })
                    .ToList(),
                Users = e.Participants.Select(participant =>
                        new RoomUserDetail { Id = participant.User.Id, Nickname = participant.User.Nickname, })
                    .ToList(),
                RoomStatus = e.Status.EnumValue,
            });
    }
}
