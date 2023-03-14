using Interview.Domain.Rooms;
using Interview.Domain.RoomUsers;
using Interview.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public class RoomTypeConfiguration : EntityTypeConfigurationBase<Room>
{
    protected override void ConfigureCore(EntityTypeBuilder<Room> builder)
    {
        builder.Property(room => room.Name).IsRequired().HasMaxLength(70);
        builder.HasMany(room => room.Questions).WithMany();
    }
}
