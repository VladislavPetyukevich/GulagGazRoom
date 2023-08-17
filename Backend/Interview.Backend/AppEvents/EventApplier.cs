using Interview.Domain.Events;
using Interview.Domain.RoomParticipants;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Backend.AppEvents;

public class EventApplier
{
    private readonly IConfiguration _configuration;

    public EventApplier(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ApplyEventsAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        var events = GetInitialEvents();
        if (events.Length == 0)
        {
            return;
        }

        var roles = db.Roles.ToDictionary(e => e.Name.EnumValue);
        var searchEvents = events.ToDictionary(e => e.Type);
        var existsEvents = GetExistsEvents(db, searchEvents);
        UpdateExistsEvents(db, existsEvents, searchEvents, roles);
        await AddEvents(db, searchEvents, existsEvents, roles, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task AddEvents(
        AppDbContext db,
        Dictionary<string, InitialEvent> searchEvents,
        Dictionary<string, AppEvent> existsEvents,
        Dictionary<RoleNameType, Role> roles,
        CancellationToken cancellationToken)
    {
        var newEvents = searchEvents.Where(e => !existsEvents.ContainsKey(e.Key)).ToList();
        foreach (var (type, initialEvent) in newEvents)
        {
            var @event = new AppEvent
            {
                Type = type,
                Roles = initialEvent.Roles.Select(e => roles[e]).ToList(),
                ParticipantTypes = AppEvent.ParseParticipantTypes(type, initialEvent.ParticipantTypes),
            };
            await db.AppEvent.AddAsync(@event, cancellationToken);
        }
    }

    private static void UpdateExistsEvents(
        AppDbContext db,
        Dictionary<string, AppEvent> existsEvents,
        Dictionary<string, InitialEvent> searchEvents,
        Dictionary<RoleNameType, Role> roles)
    {
        foreach (var (type, existsEvent) in existsEvents)
        {
            existsEvent.Roles ??= new List<Role>();
            existsEvent.ParticipantTypes ??= new List<RoomParticipantType>();
            var dbRoles = existsEvent.Roles.Select(e => e.Name.EnumValue).ToHashSet();
            dbRoles.SymmetricExceptWith(searchEvents[type].Roles);

            var existsParticipantTypes = existsEvent.ParticipantTypes.Select(e => e.Name).ToHashSet();
            existsParticipantTypes.SymmetricExceptWith(searchEvents[type].ParticipantTypes);
            if (dbRoles.Count == 0 && existsParticipantTypes.Count == 0)
            {
                continue;
            }

            var fileRoles = searchEvents[type].Roles.Select(e => roles[e]).ToList();
            existsEvent.Roles.Clear();
            existsEvent.Roles.AddRange(fileRoles);

            existsEvent.ParticipantTypes.Clear();
            existsEvent.ParticipantTypes.AddRange(AppEvent.ParseParticipantTypes(type, searchEvents[type].ParticipantTypes));
            db.AppEvent.Update(existsEvent);
        }
    }

    private static Dictionary<string, AppEvent> GetExistsEvents(AppDbContext db, Dictionary<string, InitialEvent> searchEvents)
    {
        var searchEventsTypes = searchEvents.Select(e => e.Key);
        return db.AppEvent
            .Include(e => e.Roles)
            .Where(e => searchEventsTypes.Contains(e.Type))
            .ToDictionary(e => e.Type);
    }

    private InitialEvent[] GetInitialEvents()
    {
        var initialEventsSection = _configuration.GetSection("InitialEvents");
        var events = initialEventsSection?.Get<InitialEvent[]>();
        return events ?? Array.Empty<InitialEvent>();
    }
}
