using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Interview.Infrastructure.Database.Configurations;

public class RoomConfigurationTypeConfiguration : EntityTypeConfigurationBase<Domain.Rooms.RoomConfiguration>
{
    protected override void ConfigureCore(EntityTypeBuilder<Domain.Rooms.RoomConfiguration> builder)
    {
        builder.Property(e => e.EnableCodeEditor).IsRequired();
        builder.Property(e => e.CodeEditorContent);
    }
}
