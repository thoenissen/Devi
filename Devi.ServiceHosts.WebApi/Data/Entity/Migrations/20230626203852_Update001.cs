using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Devi.ServiceHosts.WebApi.Data.Entity.Migrations
{
    /// <inheritdoc/>
    public partial class Update001 : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "LookingForGroupAppointments",
                                         columns: table => new
                                                           {
                                                               MessageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                                                               Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                                                               Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                                                               CreationUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                                                               ChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                                                               ThreadId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                                                           },
                                         constraints: table => table.PrimaryKey("PK_LookingForGroupAppointments", x => x.MessageId));

            migrationBuilder.CreateTable(name: "LookingForGroupParticipants",
                                         columns: table => new
                                                           {
                                                               AppointmentMessageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                                                               UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                                                               RegistrationTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                                                           },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_LookingForGroupParticipants",
                                                                           x => new
                                                                                {
                                                                                    x.AppointmentMessageId,
                                                                                    x.UserId
                                                                                });

                                                          table.ForeignKey(name: "FK_LookingForGroupParticipants_LookingForGroupAppointments_AppointmentMessageId",
                                                                           column: x => x.AppointmentMessageId,
                                                                           principalTable: "LookingForGroupAppointments",
                                                                           principalColumn: "MessageId",
                                                                           onDelete: ReferentialAction.Restrict);
                                                      });
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "LookingForGroupParticipants");
            migrationBuilder.DropTable(name: "LookingForGroupAppointments");
        }
    }
}