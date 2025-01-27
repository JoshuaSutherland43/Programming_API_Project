using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FIRST_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LAST_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EMAIL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PASSWORD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ROLE_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ROLE_ID1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.User_ID);
                    table.ForeignKey(
                        name: "FK_USERS_Role_ROLE_ID1",
                        column: x => x.ROLE_ID1,
                        principalTable: "Role",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CLAIM",
                columns: table => new
                {
                    CLAIM_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UPLOAD_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HOURS = table.Column<int>(type: "int", nullable: false),
                    RATE = table.Column<double>(type: "float", nullable: false),
                    NOTES = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    FILE_PATH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    STATUS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    APPROVED_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_ID1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLAIM", x => x.CLAIM_ID);
                    table.ForeignKey(
                        name: "FK_CLAIM_USERS_User_ID",
                        column: x => x.User_ID,
                        principalTable: "USERS",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HR",
                columns: table => new
                {
                    INVOICE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CLAIM_ID = table.Column<int>(type: "int", nullable: false),
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    TOTAL_AMOUNT = table.Column<double>(type: "float", nullable: false),
                    INVOICE_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CLAIM_ID1 = table.Column<int>(type: "int", nullable: false),
                    User_ID1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR", x => x.INVOICE_ID);
                    table.ForeignKey(
                        name: "FK_HR_CLAIM_CLAIM_ID1",
                        column: x => x.CLAIM_ID1,
                        principalTable: "CLAIM",
                        principalColumn: "CLAIM_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HR_USERS_User_ID",
                        column: x => x.User_ID,
                        principalTable: "USERS",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CLAIM_User_ID",
                table: "CLAIM",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_HR_CLAIM_ID1",
                table: "HR",
                column: "CLAIM_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_HR_User_ID",
                table: "HR",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_ROLE_ID1",
                table: "USERS",
                column: "ROLE_ID1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HR");

            migrationBuilder.DropTable(
                name: "CLAIM");

            migrationBuilder.DropTable(
                name: "USERS");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
