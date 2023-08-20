using CSharpFunctionalExtensions;
using Interview.Domain.Events.Service.Create;
using Interview.Domain.Events.Service.FindPage;
using Interview.Domain.Events.Service.Update;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using Interview.Domain.Users.Roles;
using NSpecifications;
using X.PagedList;

namespace Interview.Domain.Events.Service;

public class AppEventService
{
    private readonly IAppEventRepository _eventRepository;
    private readonly IRoleRepository _roleRepository;

    public AppEventService(IAppEventRepository eventRepository, IRoleRepository roleRepository)
    {
        _eventRepository = eventRepository;
        _roleRepository = roleRepository;
    }

    public async Task<IPagedList<AppEventItem>> FindPageAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var res = await _eventRepository.GetPageDetailedAsync(new AppEventItemParticipantTypeMapper(), pageNumber, pageSize, cancellationToken);
        return new StaticPagedList<AppEventItem>(res.Select(e => e.ToAppEventItem()), res);
    }

    public async Task<AppEventItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var res = await _eventRepository.FindByIdDetailedAsync(id, new AppEventItemParticipantTypeMapper(), cancellationToken);
        return res?.ToAppEventItem();
    }

    public async Task<AppEventItem?> FindByTypeAsync(string type, CancellationToken cancellationToken)
    {
        var res = await _eventRepository.FindFirstOrDefaultDetailedAsync(new Spec<AppEvent>(e => e.Type == type), new AppEventItemParticipantTypeMapper(), cancellationToken);
        return res?.ToAppEventItem();
    }

    public async Task<Result<ServiceResult<Guid>, ServiceError>> CreateAsync(
        AppEventCreateRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
        {
            return ServiceError.Error("The type cannot be empty.");
        }

        var type = request.Type.Trim();
        if (request.Roles.Count == 0)
        {
            return ServiceError.Error("At least 1 role must be specified.");
        }

        if (request.Roles.Any(e => !Enum.IsDefined(e)))
        {
            return ServiceError.Error("An unknown role is specified.");
        }

        var hasEvent = await _eventRepository.HasAsync(new Spec<AppEvent>(e => e.Type == type), cancellationToken);
        if (hasEvent)
        {
            return ServiceError.Error("An event with this type already exists.");
        }

        var requestedRoleIds = request.Roles.Select(e => RoleName.FromValue((int)e).Id).ToList();
        var roles = await _roleRepository.FindByIdsAsync(requestedRoleIds, cancellationToken);
        var newEvent = new AppEvent { Type = type, Roles = roles, ParticipantTypes = AppEvent.ParseParticipantTypes(type, request.ParticipantTypes) };
        await _eventRepository.CreateAsync(newEvent, cancellationToken);
        return ServiceResult.Created(newEvent.Id);
    }

    public async Task<Result<ServiceResult<AppEventItem>, ServiceError>> UpdateAsync(Guid id, AppEventUpdateRequest request, CancellationToken cancellationToken)
    {
        var existingEvent = await _eventRepository.FindByIdDetailedAsync(id, cancellationToken);
        if (existingEvent is null)
        {
            return ServiceError.NotFound($"Not found event by id {id}");
        }

        var mapper = new AppEventItemParticipantTypeMapper();
        if (string.IsNullOrWhiteSpace(request.Type) && (request.Roles is null || request.Roles.Count == 0))
        {
            return ServiceResult.Ok(mapper.Map(existingEvent).ToAppEventItem());
        }

        var type = request.Type?.Trim();
        if (!string.IsNullOrWhiteSpace(type))
        {
            var hasEvent = await _eventRepository.HasAsync(new Spec<AppEvent>(e => e.Type == type), cancellationToken);
            if (hasEvent)
            {
                return ServiceError.Error("An event with this type already exists.");
            }

            existingEvent.Type = type;
        }

        if (request.Roles is not null && request.Roles.Count > 0)
        {
            var requestedRoleIds = request.Roles.Select(e => RoleName.FromValue((int)e).Id).ToList();
            var roles = await _roleRepository.FindByIdsAsync(requestedRoleIds, cancellationToken);
            existingEvent.Roles = roles;
        }

        if (request.ParticipantTypes is not null && request.ParticipantTypes.Count > 0)
        {
            existingEvent.ParticipantTypes = AppEvent.ParseParticipantTypes(request.Type ?? existingEvent.Type, request.ParticipantTypes);
        }

        await _eventRepository.UpdateAsync(existingEvent, cancellationToken);
        return ServiceResult.Ok(mapper.Map(existingEvent).ToAppEventItem());
    }
}
