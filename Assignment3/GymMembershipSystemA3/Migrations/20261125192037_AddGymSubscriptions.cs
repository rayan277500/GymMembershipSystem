using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymMembershipSystemA3.Migrations
{
    /// <inheritdoc />
    public partial class AddGymSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GymSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GymMemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    MembershipPlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GymSubscriptions_GymMembers_GymMemberId",
                        column: x => x.GymMemberId,
                        principalTable: "GymMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GymSubscriptions_MembershipPlans_MembershipPlanId",
                        column: x => x.MembershipPlanId,
                        principalTable: "MembershipPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GymSubscriptions_GymMemberId",
                table: "GymSubscriptions",
                column: "GymMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_GymSubscriptions_MembershipPlanId",
                table: "GymSubscriptions",
                column: "MembershipPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GymSubscriptions");
        }
    }
}
