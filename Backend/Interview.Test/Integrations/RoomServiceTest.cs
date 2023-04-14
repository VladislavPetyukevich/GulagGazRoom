using FluentAssertions;
using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.RoomParticipants;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Interview.Infrastructure.Questions;
using Interview.Infrastructure.RoomQuestionReactions;
using Interview.Infrastructure.Rooms;
using Interview.Infrastructure.Users;

namespace Interview.Test.Integrations;

public class RoomServiceTest
{
    private const string DefaultRoomName = "Test_Room";

    [Fact(DisplayName = "Patch update room with request name not null")]
    public async Task PatchUpdateRoomWithRequestNameIsNotNull()
    {
        var testSystemClock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(testSystemClock);

        var savedRoom = new Room(DefaultRoomName, DefaultRoomName);

        appDbContext.Rooms.Add(savedRoom);

        await appDbContext.SaveChangesAsync();

        var roomRepository = new RoomRepository(appDbContext);
        var roomService = new RoomService(roomRepository, new QuestionRepository(appDbContext), new UserRepository(appDbContext), new EmptyRoomEventDispatcher(), new RoomQuestionReactionRepository(appDbContext));

        var roomPatchUpdateRequest = new RoomPatchUpdateRequest { Name = "New_Value_Name_Room", TwitchChannel = "TwitchCH" };

        var patchUpdate = await roomService.PatchUpdate(savedRoom.Id, roomPatchUpdateRequest);

        Assert.True(patchUpdate.IsSuccess);

        var foundedRoom = await roomRepository.FindByIdAsync(savedRoom.Id);

        foundedRoom?.Name.Should().BeEquivalentTo(roomPatchUpdateRequest.Name);
    }

    [Fact(DisplayName = "GetAnalytics should return valid analytics by roomId")]
    public async Task GetAnalytics_Should_Return_Valid_Analytics_By_RoomId()
    {
        var testSystemClock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(testSystemClock);

        var room1 = new Room(DefaultRoomName, DefaultRoomName);

        appDbContext.Rooms.Add(room1);
        appDbContext.Rooms.Add(new Room(DefaultRoomName + "2", DefaultRoomName + "2"));

        var questions = new Question[]
        {
            new("V1") { Id = Guid.Parse("527A0279-4364-4940-BE4E-8DBEC08BA96C") },
            new("V2") { Id = Guid.Parse("537A0279-4364-4940-BE4E-8DBEC08BA96C") },
            new("V3") { Id = Guid.Parse("547A0279-4364-4940-BE4E-8DBEC08BA96C") },
            new("V4") { Id = Guid.Parse("557A0279-4364-4940-BE4E-8DBEC08BA96C") },
            new("V5") { Id = Guid.Parse("567A0279-4364-4940-BE4E-8DBEC08BA96C") },
            new("V6") { Id = Guid.Parse("577A0279-4364-4940-BE4E-8DBEC08BA96C") }
        };
        appDbContext.Questions.AddRange(questions);

        var users = new User[]
        {
            new("u1", "v1") { Id = Guid.Parse("587A0279-4364-4940-BE4E-8DBEC08BA96C"), Roles = { appDbContext.Roles.Find(RoleName.User.Id)! } },
            new("u2", "v2") { Id = Guid.Parse("597A0279-4364-4940-BE4E-8DBEC08BA96C"), Roles = { appDbContext.Roles.Find(RoleName.Admin.Id)! } },
            new("u3", "v3") { Id = Guid.Parse("5A7A0279-4364-4940-BE4E-8DBEC08BA96C"), Roles = { appDbContext.Roles.Find(RoleName.User.Id)! } },
            new("u4", "v4") { Id = Guid.Parse("5B7A0279-4364-4940-BE4E-8DBEC08BA96C"), Roles = { appDbContext.Roles.Find(RoleName.User.Id)! } },
            new("u5", "v5") { Id = Guid.Parse("5C7A0279-4364-4940-BE4E-8DBEC08BA96C"), Roles = { appDbContext.Roles.Find(RoleName.User.Id)! } },
        };
        appDbContext.Users.AddRange(users);
        await appDbContext.SaveChangesAsync();

        var roomQuestion = new RoomQuestion[]
        {
            new() { Id = Guid.Parse("B15AA6D4-FA7B-49CB-AFA2-EA4F900F2258"), Question = questions[0], Room = room1, State = RoomQuestionState.Open },
            new() { Id = Guid.Parse("B25AA6D4-FA7B-49CB-AFA2-EA4F900F2258"), Question = questions[1], Room = room1, State = RoomQuestionState.Closed },
            new() { Id = Guid.Parse("B35AA6D4-FA7B-49CB-AFA2-EA4F900F2258"), Question = questions[2], Room = room1, State = RoomQuestionState.Closed },
            new() { Id = Guid.Parse("B45AA6D4-FA7B-49CB-AFA2-EA4F900F2258"), Question = questions[3], Room = room1, State = RoomQuestionState.Active },
        };
        appDbContext.RoomQuestions.AddRange(roomQuestion);

        var roomParticipants = new RoomParticipant[]
        {
            new(users[0], room1, RoomParticipantType.Examinee) { Id = Guid.Parse("C15AA6D4-FA7B-49CB-AFA2-EA4F900F2258") },
            new(users[1], room1, RoomParticipantType.Expert) { Id = Guid.Parse("C25AA6D4-FA7B-49CB-AFA2-EA4F900F2258") },
            new(users[2], room1, RoomParticipantType.Viewer) { Id = Guid.Parse("C35AA6D4-FA7B-49CB-AFA2-EA4F900F2258") },
            new(users[3], room1, RoomParticipantType.Viewer) { Id = Guid.Parse("C45AA6D4-FA7B-49CB-AFA2-EA4F900F2258") },
        };
        appDbContext.RoomParticipants.AddRange(roomParticipants);
        await appDbContext.SaveChangesAsync();

        var like = appDbContext.Reactions.Find(ReactionType.Like.Id) ?? throw new Exception("Unexpected state");
        var dislike = appDbContext.Reactions.Find(ReactionType.Dislike.Id) ?? throw new Exception("Unexpected state");

        var questionReactions = new RoomQuestionReaction[]
        {
            new()
            {
                Id = Guid.Parse("D15AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[0],
                Reaction = like,
                Sender = users[1],
            },
            new()
            {
                Id = Guid.Parse("D25AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[0],
                Reaction = like,
                Sender = users[1],
            },
            new()
            {
                Id = Guid.Parse("D35AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[0],
                Reaction = like,
                Sender = users[2],
            },
            new()
            {
                Id = Guid.Parse("D45AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[0],
                Reaction = dislike,
                Sender = users[3],
            },

            new()
            {
                Id = Guid.Parse("D55AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[1],
                Reaction = dislike,
                Sender = users[1],
            },
            new()
            {
                Id = Guid.Parse("D65AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[1],
                Reaction = like,
                Sender = users[2],
            },
            new()
            {
                Id = Guid.Parse("D75AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[1],
                Reaction = dislike,
                Sender = users[3],
            },
        };
        appDbContext.RoomQuestionReactions.AddRange(questionReactions);
        await appDbContext.SaveChangesAsync();

        var roomRepository = new RoomRepository(appDbContext);
        var roomService = new RoomService(roomRepository, new QuestionRepository(appDbContext), new UserRepository(appDbContext), new EmptyRoomEventDispatcher(), new RoomQuestionReactionRepository(appDbContext));

        var expectAnalytics = new Analytics
        {
            Reactions = new List<Analytics.AnalyticsReactionSummary>()
            {
                new()
                {
                    Id = ReactionType.Like.Id, Type = ReactionType.Like.Name, Count = 4
                },
                new()
                {
                    Id = ReactionType.Dislike.Id, Type = ReactionType.Dislike.Name, Count = 3
                },
            },
            Questions = new List<Analytics.AnalyticsQuestion>()
            {
                new()
                {
                    Id = questions[0].Id,
                    Value = questions[0].Value,
                    Status = RoomQuestionState.Open.Name,
                    Users = new List<Analytics.AnalyticsUser>()
                    {
                        new()
                        {
                            Id = users[1].Id,
                            Nickname = users[1].Nickname,
                            Avatar = users[1].Avatar ?? string.Empty,
                            ParticipantType = RoomParticipantType.Expert.Name,
                            Reactions = new List<Analytics.AnalyticsReaction>()
                            {
                                new() { Id = ReactionType.Like.Id, Type = ReactionType.Like.Name, CreatedAt = like.CreateDate },
                                new() { Id = ReactionType.Like.Id, Type = ReactionType.Like.Name, CreatedAt = like.CreateDate },
                            },
                            ReactionsSummary = new List<Analytics.AnalyticsReactionSummary>()
                            {
                                new()
                                {
                                    Id = ReactionType.Like.Id,
                                    Type = ReactionType.Like.Name,
                                    Count = 2,
                                }
                            },
                        },
                        new()
                        {
                            Id = users[2].Id,
                            Nickname = users[2].Nickname,
                            Avatar = users[2].Avatar ?? string.Empty,
                            ParticipantType = RoomParticipantType.Viewer.Name,
                            Reactions = new List<Analytics.AnalyticsReaction>()
                            {
                                new() { Id = ReactionType.Like.Id, Type = ReactionType.Like.Name, CreatedAt = like.CreateDate },
                            },
                            ReactionsSummary = new List<Analytics.AnalyticsReactionSummary>()
                            {
                                new()
                                {
                                    Id = ReactionType.Like.Id,
                                    Type = ReactionType.Like.Name,
                                    Count = 1,
                                }
                            },
                        },
                        new()
                        {
                            Id = users[3].Id,
                            Nickname = users[3].Nickname,
                            Avatar = users[3].Avatar ?? string.Empty,
                            ParticipantType = RoomParticipantType.Viewer.Name,
                            Reactions = new List<Analytics.AnalyticsReaction>()
                            {
                                new() { Id = ReactionType.Dislike.Id, Type = ReactionType.Dislike.Name, CreatedAt = dislike.CreateDate },
                            },
                            ReactionsSummary = new List<Analytics.AnalyticsReactionSummary>()
                            {
                                new()
                                {
                                    Id = ReactionType.Dislike.Id,
                                    Type = ReactionType.Dislike.Name,
                                    Count = 1,
                                }
                            },
                        },
                    }
                },
                new()
                {
                    Id = questions[1].Id,
                    Value = questions[1].Value,
                    Status = RoomQuestionState.Closed.Name,
                    Users = new List<Analytics.AnalyticsUser>()
                    {
                        new()
                        {
                            Id = users[1].Id,
                            Nickname = users[1].Nickname,
                            Avatar = users[1].Avatar ?? string.Empty,
                            ParticipantType = RoomParticipantType.Expert.Name,
                            Reactions = new List<Analytics.AnalyticsReaction>()
                            {
                                new() { Id = ReactionType.Dislike.Id, Type = ReactionType.Dislike.Name, CreatedAt = dislike.CreateDate },
                            },
                            ReactionsSummary = new List<Analytics.AnalyticsReactionSummary>()
                            {
                                new()
                                {
                                    Id = ReactionType.Dislike.Id,
                                    Type = ReactionType.Dislike.Name,
                                    Count = 1,
                                }
                            },
                        },
                        new()
                        {
                            Id = users[2].Id,
                            Nickname = users[2].Nickname,
                            Avatar = users[2].Avatar ?? string.Empty,
                            ParticipantType = RoomParticipantType.Viewer.Name,
                            Reactions = new List<Analytics.AnalyticsReaction>()
                            {
                                new() { Id = ReactionType.Like.Id, Type = ReactionType.Like.Name, CreatedAt = like.CreateDate },
                            },
                            ReactionsSummary = new List<Analytics.AnalyticsReactionSummary>()
                            {
                                new()
                                {
                                    Id = ReactionType.Like.Id,
                                    Type = ReactionType.Like.Name,
                                    Count = 1,
                                }
                            },
                        },
                        new()
                        {
                            Id = users[3].Id,
                            Nickname = users[3].Nickname,
                            Avatar = users[3].Avatar ?? string.Empty,
                            ParticipantType = RoomParticipantType.Viewer.Name,
                            Reactions = new List<Analytics.AnalyticsReaction>()
                            {
                                new() { Id = ReactionType.Dislike.Id, Type = ReactionType.Dislike.Name, CreatedAt = dislike.CreateDate },
                            },
                            ReactionsSummary = new List<Analytics.AnalyticsReactionSummary>()
                            {
                                new()
                                {
                                    Id = ReactionType.Dislike.Id,
                                    Type = ReactionType.Dislike.Name,
                                    Count = 1,
                                }
                            },
                        },
                    }
                },
                new()
                {
                    Id = questions[2].Id,
                    Value = questions[2].Value,
                    Status = RoomQuestionState.Closed.Name,
                },
                new()
                {
                    Id = questions[3].Id,
                    Value = questions[3].Value,
                    Status = RoomQuestionState.Active.Name,
                }
            }
        };

        var analyticsResult = await roomService.GetAnalyticsAsync(new RoomAnalyticsRequest(room1.Id));

        Assert.True(analyticsResult.IsSuccess);

        var serviceResult = analyticsResult.Value;
        serviceResult.Should().NotBeNull();
        serviceResult.Value.Should().BeEquivalentTo(expectAnalytics);
    }

    [Fact(DisplayName = "GetAnalytics should return valid analytics by roomId and userId")]
    public async Task GetAnalytics_Should_Return_Valid_Analytics_By_RoomId_And_UserId()
    {
        var testSystemClock = new TestSystemClock();
        await using var appDbContext = new TestAppDbContextFactory().Create(testSystemClock);

        var room1 = new Room(DefaultRoomName, DefaultRoomName);

        appDbContext.Rooms.Add(room1);
        appDbContext.Rooms.Add(new Room(DefaultRoomName + "2", DefaultRoomName + "2"));

        var questions = new Question[]
        {
            new("V1") { Id = Guid.Parse("527A0279-4364-4940-BE4E-8DBEC08BA96C") },
            new("V2") { Id = Guid.Parse("537A0279-4364-4940-BE4E-8DBEC08BA96C") },
            new("V3") { Id = Guid.Parse("547A0279-4364-4940-BE4E-8DBEC08BA96C") },
            new("V4") { Id = Guid.Parse("557A0279-4364-4940-BE4E-8DBEC08BA96C") },
            new("V5") { Id = Guid.Parse("567A0279-4364-4940-BE4E-8DBEC08BA96C") },
            new("V6") { Id = Guid.Parse("577A0279-4364-4940-BE4E-8DBEC08BA96C") }
        };
        appDbContext.Questions.AddRange(questions);

        var users = new User[]
        {
            new("u1", "v1") { Id = Guid.Parse("587A0279-4364-4940-BE4E-8DBEC08BA96C"), Roles = { appDbContext.Roles.Find(RoleName.User.Id)! } },
            new("u2", "v2") { Id = Guid.Parse("597A0279-4364-4940-BE4E-8DBEC08BA96C"), Roles = { appDbContext.Roles.Find(RoleName.Admin.Id)! } },
            new("u3", "v3") { Id = Guid.Parse("5A7A0279-4364-4940-BE4E-8DBEC08BA96C"), Roles = { appDbContext.Roles.Find(RoleName.User.Id)! } },
            new("u4", "v4") { Id = Guid.Parse("5B7A0279-4364-4940-BE4E-8DBEC08BA96C"), Roles = { appDbContext.Roles.Find(RoleName.User.Id)! } },
            new("u5", "v5") { Id = Guid.Parse("5C7A0279-4364-4940-BE4E-8DBEC08BA96C"), Roles = { appDbContext.Roles.Find(RoleName.User.Id)! } },
        };
        appDbContext.Users.AddRange(users);
        await appDbContext.SaveChangesAsync();

        var roomQuestion = new RoomQuestion[]
        {
            new() { Id = Guid.Parse("B15AA6D4-FA7B-49CB-AFA2-EA4F900F2258"), Question = questions[0], Room = room1, State = RoomQuestionState.Open },
            new() { Id = Guid.Parse("B25AA6D4-FA7B-49CB-AFA2-EA4F900F2258"), Question = questions[1], Room = room1, State = RoomQuestionState.Closed },
            new() { Id = Guid.Parse("B35AA6D4-FA7B-49CB-AFA2-EA4F900F2258"), Question = questions[2], Room = room1, State = RoomQuestionState.Closed },
            new() { Id = Guid.Parse("B45AA6D4-FA7B-49CB-AFA2-EA4F900F2258"), Question = questions[3], Room = room1, State = RoomQuestionState.Active },
        };
        appDbContext.RoomQuestions.AddRange(roomQuestion);

        var roomParticipants = new RoomParticipant[]
        {
            new(users[0], room1, RoomParticipantType.Examinee) { Id = Guid.Parse("C15AA6D4-FA7B-49CB-AFA2-EA4F900F2258") },
            new(users[1], room1, RoomParticipantType.Expert) { Id = Guid.Parse("C25AA6D4-FA7B-49CB-AFA2-EA4F900F2258") },
            new(users[2], room1, RoomParticipantType.Viewer) { Id = Guid.Parse("C35AA6D4-FA7B-49CB-AFA2-EA4F900F2258") },
            new(users[3], room1, RoomParticipantType.Viewer) { Id = Guid.Parse("C45AA6D4-FA7B-49CB-AFA2-EA4F900F2258") },
        };
        appDbContext.RoomParticipants.AddRange(roomParticipants);
        await appDbContext.SaveChangesAsync();

        var like = appDbContext.Reactions.Find(ReactionType.Like.Id) ?? throw new Exception("Unexpected state");
        var dislike = appDbContext.Reactions.Find(ReactionType.Dislike.Id) ?? throw new Exception("Unexpected state");

        var questionReactions = new RoomQuestionReaction[]
        {
            new()
            {
                Id = Guid.Parse("D15AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[0],
                Reaction = like,
                Sender = users[1],
            },
            new()
            {
                Id = Guid.Parse("D25AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[0],
                Reaction = like,
                Sender = users[1],
            },
            new()
            {
                Id = Guid.Parse("D35AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[0],
                Reaction = like,
                Sender = users[2],
            },
            new()
            {
                Id = Guid.Parse("D45AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[0],
                Reaction = dislike,
                Sender = users[3],
            },

            new()
            {
                Id = Guid.Parse("D55AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[1],
                Reaction = dislike,
                Sender = users[1],
            },
            new()
            {
                Id = Guid.Parse("D65AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[1],
                Reaction = like,
                Sender = users[2],
            },
            new()
            {
                Id = Guid.Parse("D75AA6D4-FA7B-49CB-AFA2-EA4F900F2258"),
                RoomQuestion = roomQuestion[1],
                Reaction = dislike,
                Sender = users[3],
            },
        };
        appDbContext.RoomQuestionReactions.AddRange(questionReactions);
        await appDbContext.SaveChangesAsync();

        var roomRepository = new RoomRepository(appDbContext);
        var roomService = new RoomService(roomRepository, new QuestionRepository(appDbContext), new UserRepository(appDbContext), new EmptyRoomEventDispatcher(), new RoomQuestionReactionRepository(appDbContext));

        var expectAnalytics = new Analytics
        {
            Reactions = new List<Analytics.AnalyticsReactionSummary>()
            {
                new()
                {
                    Id = ReactionType.Like.Id, Type = ReactionType.Like.Name, Count = 4
                },
                new()
                {
                    Id = ReactionType.Dislike.Id, Type = ReactionType.Dislike.Name, Count = 3
                },
            },
            Questions = new List<Analytics.AnalyticsQuestion>()
            {
                new()
                {
                    Id = questions[0].Id,
                    Value = questions[0].Value,
                    Status = RoomQuestionState.Open.Name,
                    Users = new List<Analytics.AnalyticsUser>()
                    {
                        new()
                        {
                            Id = users[1].Id,
                            Nickname = users[1].Nickname,
                            Avatar = users[1].Avatar ?? string.Empty,
                            ParticipantType = RoomParticipantType.Expert.Name,
                            Reactions = new List<Analytics.AnalyticsReaction>()
                            {
                                new() { Id = ReactionType.Like.Id, Type = ReactionType.Like.Name, CreatedAt = like.CreateDate },
                                new() { Id = ReactionType.Like.Id, Type = ReactionType.Like.Name, CreatedAt = like.CreateDate },
                            },
                            ReactionsSummary = new List<Analytics.AnalyticsReactionSummary>()
                            {
                                new()
                                {
                                    Id = ReactionType.Like.Id,
                                    Type = ReactionType.Like.Name,
                                    Count = 2,
                                }
                            },
                        },
                    }
                },
                new()
                {
                    Id = questions[1].Id,
                    Value = questions[1].Value,
                    Status = RoomQuestionState.Closed.Name,
                    Users = new List<Analytics.AnalyticsUser>()
                    {
                        new()
                        {
                            Id = users[1].Id,
                            Nickname = users[1].Nickname,
                            Avatar = users[1].Avatar ?? string.Empty,
                            ParticipantType = RoomParticipantType.Expert.Name,
                            Reactions = new List<Analytics.AnalyticsReaction>()
                            {
                                new() { Id = ReactionType.Dislike.Id, Type = ReactionType.Dislike.Name, CreatedAt = dislike.CreateDate },
                            },
                            ReactionsSummary = new List<Analytics.AnalyticsReactionSummary>()
                            {
                                new()
                                {
                                    Id = ReactionType.Dislike.Id,
                                    Type = ReactionType.Dislike.Name,
                                    Count = 1,
                                }
                            },
                        },
                    }
                },
                new()
                {
                    Id = questions[2].Id,
                    Value = questions[2].Value,
                    Status = RoomQuestionState.Closed.Name,
                },
                new()
                {
                    Id = questions[3].Id,
                    Value = questions[3].Value,
                    Status = RoomQuestionState.Active.Name,
                }
            }
        };

        var analyticsResult = await roomService.GetAnalyticsAsync(new RoomAnalyticsRequest(room1.Id, new List<Guid> { users[1].Id }));

        Assert.True(analyticsResult.IsSuccess);

        var serviceResult = analyticsResult.Value;
        serviceResult.Should().NotBeNull();
        serviceResult.Value.Should().BeEquivalentTo(expectAnalytics);
    }
}
