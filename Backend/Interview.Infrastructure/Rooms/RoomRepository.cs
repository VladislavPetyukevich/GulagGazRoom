using Interview.Domain.RoomParticipants;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response.Detail;
using Interview.Domain.Users;
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
        var analytics = await GetAnalyticsCoreAsync(cancellationToken);
        if (analytics == null)
        {
            return null;
        }

        var reactions = await GetRoomQuestionReactionAsync(cancellationToken);

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

        Task<List<RoomQuestionReaction>> GetRoomQuestionReactionAsync(CancellationToken ct)
        {
            return Db.RoomQuestionReactions.AsNoTracking()
                .Include(e => e.Sender)
                .Include(e => e.Reaction)
                .Include(e => e.RoomQuestion)
                    .ThenInclude(e => e.Question)
                .Where(e => e.RoomQuestion.Room.Id == roomId)
                .ToListAsync(ct);
        }

        Task<Analytics?> GetAnalyticsCoreAsync(CancellationToken ct)
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
                .Where(e => e.Room.Id == roomId && users.Contains(e.User.Id))
                .ToDictionaryAsync(e => e.User.Id, cancellationToken);

            return roomReactions
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

    public async Task<AnalyticsSummary?> GetAnalyticsSummaryAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var reactions = Db.RoomQuestionReactions
            .Include(e => e.RoomQuestion)
                .ThenInclude(e => e.Room)
                .ThenInclude(e => e.Questions)
            .Include(e => e.Sender)
            .Include(e => e.Reaction)
            .Where(e => e.RoomQuestion.Room.Id == roomId)
            .ToLookup(e => e.RoomQuestion!, e => e);

        var participants = await Db.RoomParticipants.AsNoTracking()
            .Include(e => e.User)
            .Where(e => e.Room.Id == roomId)
            .ToDictionaryAsync(e => e.User.Id, e => e.Type, cancellationToken);

        var questions = reactions.OrderBy(e => e.Key.Question.Value)
            .Select(e => new AnalyticsSummaryQuestion
            {
                Id = e.Key.Question!.Id,
                Value = e.Key.Question.Value,
                Experts = GetExperts(e).ToList(),
                Viewers = GetViewers(e).ToList(),
            });

        return new AnalyticsSummary { Questions = questions.ToList(), };

        IEnumerable<AnalyticsSummaryExpert> GetExperts(IGrouping<RoomQuestion, RoomQuestionReaction> grouping)
        {
            var experts = grouping.Select(e => e.Sender).Distinct()
                .Where(e => participants[e.Id] == RoomParticipantType.Expert)
                .ToList();

            foreach (var analyticsSummaryExpert in experts)
            {
                var reactionsOfParticipant = grouping.Where(e => e.Sender == analyticsSummaryExpert)
                    .GroupBy(e => (e.Reaction.Type, e.Reaction.Id))
                    .Select(e => new Analytics.AnalyticsReactionSummary
                    {
                        Id = e.Key.Id,
                        Type = e.Key.Type.Name,
                        Count = e.Count(),
                    });

                yield return new AnalyticsSummaryExpert
                {
                    Nickname = analyticsSummaryExpert.Nickname,
                    ReactionsSummary = reactionsOfParticipant.ToList(),
                };
            }
        }

        IEnumerable<AnalyticsSummaryViewer> GetViewers(IGrouping<RoomQuestion, RoomQuestionReaction> grouping)
        {
            var experts = grouping.Select(e => e.Sender).Distinct()
                .Where(e => participants[e.Id] == RoomParticipantType.Viewer)
                .ToList();

            foreach (var analyticsSummaryExpert in experts)
            {
                var reactionsOfParticipant = grouping.Where(e => e.Sender == analyticsSummaryExpert)
                    .GroupBy(e => (e.Reaction.Type, e.Reaction.Id))
                    .Select(e => new Analytics.AnalyticsReactionSummary
                    {
                        Id = e.Key.Id,
                        Type = e.Key.Type.Name,
                        Count = e.Count(),
                    });

                yield return new AnalyticsSummaryViewer
                {
                    ReactionsSummary = reactionsOfParticipant.ToList(),
                };
            }
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
