using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devi.ServiceHosts.WebApi.Data.Entity.Migrations
{
    /// <inheritdoc/>
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "OneTimeReminders",
                                         columns: table => new
                                                           {
                                                               Id = table.Column<long>(type: "bigint", nullable: false)
                                                                         .Annotation("SqlServer:Identity", "1, 1"),
                                                               DiscordUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                                                               DiscordChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                                                               TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                                                               Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                                                               IsExecuted = table.Column<bool>(type: "bit", nullable: false)
                                                           },
                                         constraints: table => table.PrimaryKey("PK_OneTimeReminders", x => x.Id));
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OneTimeReminders");
        }
    }
}