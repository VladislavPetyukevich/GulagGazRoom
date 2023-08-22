using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.Users;
using Interview.Domain.Users.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User = TwitchLib.PubSub.Models.Responses.Messages.User;

namespace Interview.Infrastructure.Database.Configurations;

public class PermissionConfiguration : EntityTypeConfigurationBase<Permission>
{
    private const string QuestionResource = nameof(Question);
    private const string ReactionResource = nameof(Reaction);

    protected override void ConfigureCore(EntityTypeBuilder<Permission> builder)
    {
        builder.Property(e => e.Type)
            .HasConversion(permissionType => permissionType.Name, name => PermissionNameType.FromName(name, false))
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.Resource)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(e => new { e.Type, e.Resource }).IsUnique();

        var questionPermissions = new[]
        {
            new Permission(
                Guid.Parse("a151edb4-ea1b-401e-8294-02666c8a39f4"),
                PermissionNameType.Write,
                QuestionResource),
            new Permission(
                Guid.Parse("a8fc4913-7ddf-4a78-930f-e67af63ac436"),
                PermissionNameType.Modify,
                QuestionResource),
        }.ToList();

        var reactionPermissions = new[]
        {
            new Permission(
                Guid.Parse("e3868093-af5c-477b-90de-95c04e857ce9"),
                PermissionNameType.Write,
                ReactionResource),
            new Permission(
                Guid.Parse("22e9e2a9-aeb2-4476-b548-dffdcbfe5d22"),
                PermissionNameType.Modify,
                ReactionResource),
        }.ToList();

        var permissions = questionPermissions.Concat(reactionPermissions).ToList();

        foreach (var permission in permissions)
        {
            permission.UpdateCreateDate(new DateTime(2023, 08, 20));
        }

        builder.HasData(permissions);
    }
}
