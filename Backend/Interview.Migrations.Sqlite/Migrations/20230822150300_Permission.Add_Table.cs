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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionUser");

            migrationBuilder.DropTable(
                name: "Permissions");
        }
    }
}
