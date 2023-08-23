using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Interview.Migrations.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class PermissionAdd_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Rooms",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "RoomReview",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "RoomQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "RoomQuestionReactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "RoomParticipants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "RoomConfiguration",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Roles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Reactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Questions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Resource = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PermissionUser",
                columns: table => new
                {
                    PermissionsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionUser", x => new { x.PermissionsId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PermissionUser_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreateDate", "CreatedById", "Resource", "Type", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("22e9e2a9-aeb2-4476-b548-dffdcbfe5d22"), new DateTime(2023, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Reaction", "modify", new DateTime(2023, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("a151edb4-ea1b-401e-8294-02666c8a39f4"), new DateTime(2023, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Question", "write", new DateTime(2023, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("a8fc4913-7ddf-4a78-930f-e67af63ac436"), new DateTime(2023, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Question", "modify", new DateTime(2023, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("e3868093-af5c-477b-90de-95c04e857ce9"), new DateTime(2023, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Reaction", "write", new DateTime(2023, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.UpdateData(
                table: "Reactions",
                keyColumn: "Id",
                keyValue: new Guid("48bfc63a-9498-4438-9211-d2c29d6b3a93"),
                columns: new[] { "CreateDate", "CreatedById", "UpdateDate" },
                values: new object[] { new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Reactions",
                keyColumn: "Id",
                keyValue: new Guid("d9a79bbb-5cbb-43d4-80fb-c490e91c333c"),
                columns: new[] { "CreateDate", "CreatedById", "UpdateDate" },
                values: new object[] { new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ab45a82b-aa1c-11ed-abe8-f2b335a02ee9"),
                columns: new[] { "CreateDate", "CreatedById", "UpdateDate" },
                values: new object[] { new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ab45cf57-aa1c-11ed-970f-98dc442de35a"),
                columns: new[] { "CreateDate", "CreatedById", "UpdateDate" },
                values: new object[] { new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedById",
                table: "Users",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CreatedById",
                table: "Rooms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReview_CreatedById",
                table: "RoomReview",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomQuestions_CreatedById",
                table: "RoomQuestions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomQuestionReactions_CreatedById",
                table: "RoomQuestionReactions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomParticipants_CreatedById",
                table: "RoomParticipants",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoomConfiguration_CreatedById",
                table: "RoomConfiguration",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedById",
                table: "Roles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_CreatedById",
                table: "Reactions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CreatedById",
                table: "Questions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CreatedById",
                table: "Permissions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Type_Resource",
                table: "Permissions",
                columns: new[] { "Type", "Resource" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionUser_UserId",
                table: "PermissionUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_CreatedById",
                table: "Questions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reactions_Users_CreatedById",
                table: "Reactions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_CreatedById",
                table: "Roles",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomConfiguration_Users_CreatedById",
                table: "RoomConfiguration",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomParticipants_Users_CreatedById",
                table: "RoomParticipants",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomQuestionReactions_Users_CreatedById",
                table: "RoomQuestionReactions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomQuestions_Users_CreatedById",
                table: "RoomQuestions",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomReview_Users_CreatedById",
                table: "RoomReview",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_CreatedById",
                table: "Rooms",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedById",
                table: "Users",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_CreatedById",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_Users_CreatedById",
                table: "Reactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_CreatedById",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomConfiguration_Users_CreatedById",
                table: "RoomConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomParticipants_Users_CreatedById",
                table: "RoomParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomQuestionReactions_Users_CreatedById",
                table: "RoomQuestionReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomQuestions_Users_CreatedById",
                table: "RoomQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomReview_Users_CreatedById",
                table: "RoomReview");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_CreatedById",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedById",
                table: "Users");

            migrationBuilder.DropTable(
                name: "PermissionUser");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedById",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_CreatedById",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_RoomReview_CreatedById",
                table: "RoomReview");

            migrationBuilder.DropIndex(
                name: "IX_RoomQuestions_CreatedById",
                table: "RoomQuestions");

            migrationBuilder.DropIndex(
                name: "IX_RoomQuestionReactions_CreatedById",
                table: "RoomQuestionReactions");

            migrationBuilder.DropIndex(
                name: "IX_RoomParticipants_CreatedById",
                table: "RoomParticipants");

            migrationBuilder.DropIndex(
                name: "IX_RoomConfiguration_CreatedById",
                table: "RoomConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_Roles_CreatedById",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Reactions_CreatedById",
                table: "Reactions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CreatedById",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RoomReview");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RoomQuestions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RoomQuestionReactions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RoomParticipants");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RoomConfiguration");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Questions");

            migrationBuilder.UpdateData(
                table: "Reactions",
                keyColumn: "Id",
                keyValue: new Guid("48bfc63a-9498-4438-9211-d2c29d6b3a93"),
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Reactions",
                keyColumn: "Id",
                keyValue: new Guid("d9a79bbb-5cbb-43d4-80fb-c490e91c333c"),
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ab45a82b-aa1c-11ed-abe8-f2b335a02ee9"),
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ab45cf57-aa1c-11ed-970f-98dc442de35a"),
                columns: new[] { "CreateDate", "UpdateDate" },
                values: new object[] { new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
