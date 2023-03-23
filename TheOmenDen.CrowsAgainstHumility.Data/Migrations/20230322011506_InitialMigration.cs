using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheOmenDen.CrowsAgainstHumility.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fn_MultiFilterWordsForCardsResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Packs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsOfficialPack = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlackCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PickAnswersCount = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))"),
                    PackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackCards_Id", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_BlackCards_Packs",
                        column: x => x.PackId,
                        principalTable: "Packs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WhiteCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newsequentialid())"),
                    CardText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhiteCards_Id", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_WhiteCards_Packs",
                        column: x => x.PackId,
                        principalTable: "Packs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlackCards_PackId",
                table: "BlackCards",
                column: "PackId");

            migrationBuilder.CreateIndex(
                name: "IX_Packs_IsOfficialPack",
                table: "Packs",
                column: "IsOfficialPack");

            migrationBuilder.CreateIndex(
                name: "IX_Packs_Name",
                table: "Packs",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_WhiteCards_PackId",
                table: "WhiteCards",
                column: "PackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlackCards");

            migrationBuilder.DropTable(
                name: "fn_MultiFilterWordsForCardsResult");

            migrationBuilder.DropTable(
                name: "WhiteCards");

            migrationBuilder.DropTable(
                name: "Packs");
        }
    }
}
