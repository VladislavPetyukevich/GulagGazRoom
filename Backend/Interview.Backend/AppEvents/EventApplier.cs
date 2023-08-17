using Interview.Domain.Events;
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
        var searchEvents = events.ToDictionary(e => e.Type, e => e.Roles);
        var existsEvents = GetExistsEvents(db, searchEvents);
        UpdateEvents(db, existsEvents, searchEvents, roles);
        await AddEvents(db, searchEvents, existsEvents, roles, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task AddEvents(
        AppDbContext db,
        Dictionary<string, RoleNameType[]> searchEvents,
        Dictionary<string, AppEvent> existsEvents,
        Dictionary<RoleNameType, Role> roles,
        CancellationToken cancellationToken)
    {
        var newEvents = searchEvents.Where(e => !existsEvents.ContainsKey(e.Key)).ToList();
        foreach (var (type, newEventRoles) in newEvents)
        {
            var @event = new AppEvent
            {
                Type = type,
                Roles = newEventRoles.Select(e => roles[e]).ToList(),
            };
            await db.AppEvent.AddAsync(@event, cancellationToken);
        }
    }

    private static void UpdateEvents(
        AppDbContext db,
        Dictionary<string, AppEvent> existsEvents,
        Dictionary<string, RoleNameType[]> searchEvents,
        Dictionary<RoleNameType, Role> roles)
    {
        foreach (var (type, existsEvent) in existsEvents)
        {
            var dbRoles = existsEvent.Roles.Select(e => e.Name.EnumValue).ToHashSet();
            dbRoles.SymmetricExceptWith(searchEvents[type]);
            if (dbRoles.Count == 0)
            {
                continue;
            }

            var fileRoles = searchEvents[type].Select(e => roles[e]).ToList();
            existsEvent.Roles.Clear();
            existsEvent.Roles.AddRange(fileRoles);
            db.AppEvent.Update(existsEvent);
        }
    }

    private static Dictionary<string, AppEvent> GetExistsEvents(AppDbContext db, Dictionary<string, RoleNameType[]> searchEvents)
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
