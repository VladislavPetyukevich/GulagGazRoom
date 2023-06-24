using CSharpFunctionalExtensions;
using Interview.Domain.RoomReviews.Mappers;
using Interview.Domain.RoomReviews.Records;
using Interview.Domain.Rooms;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using Interview.Domain.Users;
using NSpecifications;
using X.PagedList;

namespace Interview.Domain.RoomReviews;

public class RoomReviewService
{
    private readonly IRoomReviewRepository _roomReviewRepository;

    private readonly IRoomRepository _roomRepository;

    private readonly IUserRepository _userRepository;

    public RoomReviewService(
        IRoomReviewRepository roomReviewRepository,
        IUserRepository userRepository,
        IRoomRepository roomRepository)
    {
        _roomReviewRepository = roomReviewRepository;
        _userRepository = userRepository;
        _roomRepository = roomRepository;
    }

    public Task<IPagedList<RoomReviewDetail>> FindPageAsync(
        RoomReviewPageRequest request,
        CancellationToken cancellationToken = default)
    {
        var specification = Spec<RoomReview>.Any;
        if (request.Filter.RoomId is not null)
        {
            specification &= new Spec<RoomReview>(review => review.Room!.Id == request.Filter.RoomId);
        }

        if (request.Filter.State is not null)
        {
            var state = SERoomReviewState.FromEnum(request.Filter.State.Value);
            specification &= new Spec<RoomReview>(review => review.SeRoomReviewState == state);
        }

        return _roomReviewRepository.GetPageDetailedAsync(
            specification,
            RoomReviewDetailMapper.Instance,
            request.Page.PageNumber,
            request.Page.PageSize,
            cancellationToken);
    }

    public async Task<Result<ServiceResult<RoomReviewDetail>, ServiceError>> CreateAsync(
        RoomReviewCreateRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindByIdAsync(userId, cancellationToken);

        if (user == null)
        {
            return ServiceError.NotFound($"User not found with id {userId}");
        }

        var room = await _roomRepository.FindByIdAsync(request.RoomId, cancellationToken);

        if (room == null)
        {
            return ServiceError.NotFound($"Room not found with id {request.RoomId}");
        }

        if (room.Status != SERoomStatus.Review)
        {
            return ServiceError.Error("Room should be in Review status");
        }

        var roomReview = new RoomReview(user, room, SERoomReviewState.Open)
        {
            Review = request.Review,
        };

        await _roomReviewRepository.CreateAsync(roomReview, cancellationToken);

        return ServiceResult.Ok(RoomReviewDetailMapper.Instance.Map(roomReview));
    }

    public async Task<Result<ServiceResult<RoomReviewDetail>, ServiceError>> UpdateAsync(
        Guid id, RoomReviewUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var roomReview = await _roomReviewRepository.FindByIdAsync(id, cancellationToken);

        if (roomReview == null)
        {
            return ServiceError.NotFound($"Review not found with id {id}");
        }

        roomReview.Review = request.Review;

        var state = SERoomReviewState.FromEnum(request.State);

        if (state == null)
        {
            return ServiceError.NotFound($"State not found with value {request.State}");
        }

        roomReview.SeRoomReviewState = state;

        await _roomReviewRepository.UpdateAsync(roomReview, cancellationToken);

        return ServiceResult.Ok(RoomReviewDetailMapper.Instance.Map(roomReview));
    }
}
