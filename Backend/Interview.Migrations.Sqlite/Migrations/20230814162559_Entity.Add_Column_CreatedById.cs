using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Interview.Migrations.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class EntityAdd_Column_CreatedById : Migration
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
