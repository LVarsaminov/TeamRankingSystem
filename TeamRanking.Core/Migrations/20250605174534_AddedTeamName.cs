using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamRanking.Core.Migrations
{
    public partial class AddedTeamName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DatePlayed",
                table: "Matches",
                newName: "MatchDate");

            migrationBuilder.AddColumn<string>(
                name: "Team1Name",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Team2Name",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Team1Name",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Team2Name",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "MatchDate",
                table: "Matches",
                newName: "DatePlayed");
        }
    }
}
