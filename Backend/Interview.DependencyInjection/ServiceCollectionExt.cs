using Interview.Domain.Certificates;
using Interview.Domain.Events;
using Interview.Domain.Events.ChangeEntityProcessors;
using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.RoomParticipants;
using Interview.Domain.RoomParticipants.Service;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Interview.Infrastructure.Certificates.Pdf;
using Interview.Infrastructure.Database;
using Interview.Infrastructure.Questions;
using Interview.Infrastructure.Reactions;
using Interview.Infrastructure.RoomParticipants;
using Interview.Infrastructure.RoomQuestionReactions;
using Interview.Infrastructure.RoomQuestions;
using Interview.Infrastructure.Rooms;
using Interview.Infrastructure.Users;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace Interview.DependencyInjection;

public static class ServiceCollectionExt
{
    public static IServiceCollection AddAppServices(this IServiceCollection self, DependencyInjectionAppServiceOption option)
    {
        self.AddSingleton<Func<AppDbContext>>(provider => () => ActivatorUtilities.CreateInstance<AppDbContext>(provider));
#pragma warning disable EF1001
        self.AddSingleton<IDbContextPool<AppDbContext>, AppDbContextPool<AppDbContext>>();
#pragma warning restore EF1001
        self.AddDbContextPool<AppDbContext>(option.DbConfigurator);

        self.AddScoped<IUserRepository, UserRepository>();
        self.AddScoped<IRoomRepository, RoomRepository>();
        self.AddScoped<IQuestionRepository, QuestionRepository>();
        self.AddScoped<IRoleRepository, RoleRepository>();
        self.AddScoped<IRoomParticipantRepository, RoomParticipantRepository>();
        self.AddScoped<IRoomQuestionRepository, RoomQuestionRepository>();
        self.AddScoped<IReactionRepository, ReactionRepository>();
        self.AddScoped<IRoomQuestionReactionRepository, RoomQuestionReactionRepository>();

        self.AddSingleton<ICertificateGenerator, PdfCertificateGenerator>();
        self.AddSingleton<IRoomEventDispatcher, RoomEventDispatcher>();
        self.AddSingleton<ISystemClock, SystemClock>();
        self.AddSingleton(option.AdminUsers);

        self.AddSingleton<IChangeEntityProcessor, RoomQuestionReactionChangeEntityProcessor>();
        self.AddSingleton<IChangeEntityProcessor, QuestionChangeEntityProcessor>();
        self.AddSingleton<IChangeEntityProcessor, RoomQuestionChangeEntityProcessor>();

        // Services
        self.AddScoped<UserService>();
        self.AddScoped<RoomService>();
        self.AddScoped<QuestionService>();
        self.AddScoped<RoomParticipantService>();
        self.AddScoped<RoomQuestionService>();
        self.AddScoped<RoomQuestionReactionService>();
        self.AddScoped<ReactionService>();

        self.AddSingleton(option.TwitchTokenProviderOption);

        return self;
    }
}
