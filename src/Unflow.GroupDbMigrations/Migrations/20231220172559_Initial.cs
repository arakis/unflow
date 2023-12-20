using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Unflow.DbGroupMigrations.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    References = table.Column<string>(type: "TEXT", nullable: false),
                    MessageId = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Author = table.Column<string>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    ArticleNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    HeaderDownloadedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    BodyDownloadedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    BlobId = table.Column<int>(type: "INTEGER", nullable: false),
                    BlogOffset = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_Id",
                table: "Article",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Article_MessageId",
                table: "Article",
                column: "MessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Article");
        }
    }
}
