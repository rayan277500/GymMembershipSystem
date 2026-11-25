using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMembershipSystemA3.Migrations
{
    /// <inheritdoc />
    public partial class AddGymMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GymMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymMembers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GymMembers_Email",
                table: "GymMembers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GymMembers_PhoneNumber",
                table: "GymMembers",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GymMembers");
        }
    }
}
