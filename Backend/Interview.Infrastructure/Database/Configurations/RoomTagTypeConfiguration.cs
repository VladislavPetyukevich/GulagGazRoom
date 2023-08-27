using Interview.Domain.Rooms;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public class RoomTagTypeConfiguration : TagLinkTypeConfigurationBase<RoomTag>
{
    protected override void ConfigureCore(EntityTypeBuilder<RoomTag> builder)
    {
        builder.HasIndex(e => e.RoomId);
        builder.HasOne(e => e.Room)
            .WithMany(e => e.Tags)
            .HasForeignKey(e => e.RoomId)
            .IsRequired();
        Configure(builder, e => e.RoomTags);
    }
}
