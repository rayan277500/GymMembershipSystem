using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMembershipSystemA3.Migrations
{
    /// <inheritdoc />
    public partial class AddGymVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GymVisits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GymMemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckInAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CheckOutAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GymVisits_GymMembers_GymMemberId",
                        column: x => x.GymMemberId,
                        principalTable: "GymMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GymVisits_GymMemberId",
                table: "GymVisits",
                column: "GymMemberId",
                unique: true,
                filter: "\"CheckOutAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GymVisits");
        }
    }
}
