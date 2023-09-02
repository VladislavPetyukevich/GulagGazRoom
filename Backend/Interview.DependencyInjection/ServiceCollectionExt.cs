using Interview.Domain;
using Interview.Domain.Certificates;
using Interview.Domain.Connections;
using Interview.Domain.Events;
using Interview.Domain.Events.ChangeEntityProcessors;
using Interview.Domain.Events.Events.Serializers;
using Interview.Domain.Permissions;
using Interview.Domain.Questions.Permissions;
using Interview.Domain.Questions.Services;
using Interview.Domain.Reactions;
using Interview.Domain.Reactions.Permissions;
using Interview.Domain.Reactions.Services;
using Interview.Domain.Repository;
using Interview.Domain.RoomParticipants.Permissions;
using Interview.Domain.RoomParticipants.Service;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestionReactions.Permissions;
using Interview.Domain.RoomQuestionReactions.Services;
using Interview.Domain.RoomQuestions.Permissions;
using Interview.Domain.RoomQuestions.Services;
using Interview.Domain.RoomReviews.Permissions;
using Interview.Domain.RoomReviews.Services;
using Interview.Domain.Rooms.Permissions;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Users;
using Interview.Domain.Users.Permissions;
using Interview.Infrastructure.Certificates.Pdf;
using Interview.Infrastructure.Database;
using Interview.Infrastructure.Database.Processors;
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
        self.AddScoped<IPooledDbContextInterceptor<AppDbContext>, UserAccessorDbContextInterceptor>();
#pragma warning restore EF1001
        self.AddDbContextPool<AppDbContext>(option.DbConfigurator);

        self.AddSingleton<ICertificateGenerator, PdfCertificateGenerator>();
        self.AddSingleton<IRoomEventDispatcher, RoomEventDispatcher>();
        self.AddSingleton<ISystemClock, SystemClock>();
        self.AddSingleton(option.AdminUsers);

        self.AddSingleton<IConnectUserSource, ConnectUserSource>();
        self.AddSingleton<IRoomEventSerializer, JsonRoomEventSerializer>();

        // Services
        self.AddScoped<ISecurityService, SecurityService>();

        self.AddScoped<IUserService, UserService>();
        self.Decorate<IUserService, UserServicePermissionAccessor>();

        self.AddScoped<IRoomService, RoomService>();
        self.Decorate<IRoomService, RoomServicePermissionAccessor>();

        self.AddScoped<IQuestionService, QuestionService>();
        self.Decorate<IQuestionService, QuestionServicePermissionAccessor>();

        self.AddScoped<IRoomReviewService, RoomReviewService>();
        self.Decorate<IRoomReviewService, RoomReviewServicePermissionAccessor>();

        self.AddScoped<IRoomParticipantService, RoomParticipantService>();
        self.Decorate<IRoomParticipantService, RoomParticipantServicePermissionAccessor>();

        self.AddScoped<IRoomQuestionService, RoomQuestionService>();
        self.Decorate<IRoomQuestionService, RoomQuestionServicePermissionAccessor>();

        self.AddScoped<IRoomQuestionReactionService, RoomQuestionReactionService>();
        self.Decorate<IRoomQuestionReactionService, RoomQuestionReactionServicePermissionAccessor>();

        self.AddScoped<IReactionService, ReactionService>();
        self.Decorate<IReactionService, ReactionServicePermissionAccessor>();

        self.AddScoped(typeof(ArchiveService<>));

        self.AddSingleton(option.TwitchTokenProviderOption);

        self.Scan(selector =>
        {
            selector.FromAssemblies(typeof(UserRepository).Assembly, typeof(RoomQuestionReactionChangeEntityProcessor).Assembly)
                .AddClasses(filter => filter.AssignableTo(typeof(IRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                
                .AddClasses(filter => filter.AssignableTo<IEntityPostProcessor>())
                .As<IEntityPostProcessor>()
                .WithSingletonLifetime();
        });

        self.AddScoped<CurrentUserAccessor>();
        self.AddScoped<IEditableCurrentUserAccessor>(provider => provider.GetRequiredService<CurrentUserAccessor>());
        self.Decorate<IEditableCurrentUserAccessor, CachedCurrentUserAccessor>();
        self.AddScoped<ICurrentUserAccessor>(e => e.GetRequiredService<IEditableCurrentUserAccessor>());

        self.AddScoped<ICurrentPermissionAccessor, CurrentPermissionAccessor>();

        self.AddScoped<IEntityPreProcessor, DateEntityPreProcessor>();

        return self;
    }
}
