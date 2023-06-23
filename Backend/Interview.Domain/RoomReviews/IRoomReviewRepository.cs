using Interview.Domain.Repository;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using X.PagedList;

namespace Interview.Domain.RoomReviews;

public interface IRoomReviewRepository : IRepository<RoomReview>
{
    Task<IPagedList<RoomReviewPageDetail>> GetDetailedPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
