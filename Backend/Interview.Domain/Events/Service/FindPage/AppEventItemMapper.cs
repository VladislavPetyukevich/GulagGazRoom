using Interview.Domain.Repository;

namespace Interview.Domain.Events.Service.FindPage
{
    public sealed class AppEventItemMapper : Mapper<AppEvent, AppEventItem>
    {
        public AppEventItemMapper()
            : base(e => new AppEventItem
            {
                Id = e.Id,
                Type = e.Type,
                Roles = e.Roles.Select(e => e.Name.EnumValue).ToList(),
            })
        {
        }
    }
}
