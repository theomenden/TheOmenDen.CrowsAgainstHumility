using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheOmenDen.CrowsAgainstHumility.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AdjustUserLoginSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrowGamePacks",
                schema: "Crows");

            migrationBuilder.DropTable(
                name: "CrowGamePlayer");

            migrationBuilder.DropTable(
                name: "CrowGames",
                schema: "Crows");

            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "AspNetUsers");

            migrationBuilder.EnsureSchema(
                name: "Security");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                newName: "IX_UserLogins_UserId");

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    FromUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "Security",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderEmail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_AspNetUserLogins_LoginProvider_ProviderKey",
                        columns: x => new { x.LoginProvider, x.ProviderKey },
                        principalTable: "AspNetUserLogins",
                        principalColumns: new[] { "LoginProvider", "ProviderKey" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CawChat_CreatedAt",
                table: "ChatMessages",
                column: "CreatedAt")
                .Annotation("SqlServer:Include", new[] { "ToUserId", "FromUserId", "Message" });

            migrationBuilder.CreateIndex(
                name: "IX_CawChat_FromUserId",
                table: "ChatMessages",
                column: "FromUserId")
                .Annotation("SqlServer:Include", new[] { "Message" });

            migrationBuilder.CreateIndex(
                name: "IX_CawChat_ToUserId",
                table: "ChatMessages",
                column: "ToUserId")
                .Annotation("SqlServer:Include", new[] { "Message" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "Security");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogins_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.AddColumn<int>(
                name: "NotificationType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CrowGames",
                schema: "Crows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    CrowGamePackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentPlayersInGame = table.Column<int>(type: "int", nullable: false),
                    LobbyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrowGames", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "CrowGamePacks",
                schema: "Crows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    CrowGameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrowGamePack", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_CrowGamePack_CrowGames_CrowGameId",
                        column: x => x.CrowGameId,
                        principalSchema: "Crows",
                        principalTable: "CrowGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrowGamePlayer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrowGamePlayer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrowGamePlayer_AspNetUsers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrowGamePlayer_CrowGames_GameId",
                        column: x => x.GameId,
                        principalSchema: "Crows",
                        principalTable: "CrowGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrowGamePacks_CrowGameId",
                schema: "Crows",
                table: "CrowGamePacks",
                column: "CrowGameId");

            migrationBuilder.CreateIndex(
                name: "IX_CrowGamePlayer_GameId",
                table: "CrowGamePlayer",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_CrowGamePlayer_PlayerId",
                table: "CrowGamePlayer",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_CrowGames_LobbyCode",
                schema: "Crows",
                table: "CrowGames",
                column: "LobbyCode")
                .Annotation("SqlServer:Include", new[] { "Name", "CurrentPlayersInGame" });
        }
    }
}
