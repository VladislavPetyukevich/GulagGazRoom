using Interview.Domain;
using Interview.Domain.Certificates;
using Interview.Domain.Connections;
using Interview.Domain.Events;
using Interview.Domain.Events.ChangeEntityProcessors;
using Interview.Domain.Events.Events.Serializers;
using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.Repository;
using Interview.Domain.RoomParticipants.Service;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestions;
using Interview.Domain.RoomReviews;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Tags;
using Interview.Domain.Users;
using Interview.Domain.Users.Service;
using Interview.Infrastructure.Certificates.Pdf;
using Interview.Infrastructure.Database;
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
        self.AddScoped<IScopedDbContextLease<AppDbContext>, AppScopedDbContextLease<AppDbContext>>();
        self.AddScoped<IPooledDbContextInterceptor<AppDbContext>, DefaultAppDbContextPooledDbContextInterceptor>();
#pragma warning restore EF1001
        self.AddDbContextPool<AppDbContext>(option.DbConfigurator);

        self.AddSingleton<ICertificateGenerator, PdfCertificateGenerator>();
        self.AddSingleton<IRoomEventDispatcher, RoomEventDispatcher>();
        self.AddSingleton<ISystemClock, SystemClock>();
        self.AddSingleton(option.AdminUsers);

        self.AddSingleton<IChangeEntityProcessor, RoomQuestionReactionChangeEntityProcessor>();
        self.AddSingleton<IChangeEntityProcessor, QuestionChangeEntityProcessor>();
        self.AddSingleton<IChangeEntityProcessor, RoomQuestionChangeEntityProcessor>();
        self.AddSingleton<IChangeEntityProcessor, RoomConfigurationChangeEntityProcessor>();
        self.AddSingleton<IChangeEntityProcessor, RoomChangeEntityProcessor>();

        self.AddSingleton<IConnectUserSource, ConnectUserSource>();
        self.AddSingleton<IRoomEventSerializer, JsonRoomEventSerializer>();

        // Services
        self.AddScoped<UserService>();
        self.AddScoped<RoomService>();
        self.AddScoped<QuestionService>();
        self.AddScoped<TagService>();
        self.AddScoped<RoomReviewService>();
        self.AddScoped<RoomParticipantService>();
        self.AddScoped<RoomQuestionService>();
        self.AddScoped<RoomQuestionReactionService>();
        self.AddScoped<ReactionService>();
        self.AddScoped(typeof(ArchiveService<>));

        self.AddSingleton(option.TwitchTokenProviderOption);

        self.Scan(selector =>
        {
            selector.FromAssemblies(typeof(UserRepository).Assembly)
                .AddClasses(filter => filter.AssignableTo(typeof(IRepository<>)))
                .AsImplementedInterfaces();
        });

        return self;
    }
}
