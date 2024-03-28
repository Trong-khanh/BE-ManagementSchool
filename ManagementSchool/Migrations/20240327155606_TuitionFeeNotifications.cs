using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementSchool.Migrations
{
    public partial class TuitionFeeNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "369be6f5-ba37-4fb8-b2f0-fe33f58ccff9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5412561e-9a90-44b8-af53-6d51a3921702");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "61bb8ab6-0bd6-4920-b230-ced1554e997d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "949e2998-36ed-42f0-a63f-fa0c019ffd0c");

            migrationBuilder.CreateTable(
                name: "TuitionFeeNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuitionFeeNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TuitionFeeNotifications_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "ParentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "06c55c8a-e840-49af-a0b2-1820bb96c631", "2", "Student", "STUDENT" },
                    { "1f5a71f0-af09-42ab-a532-7d417f6d3a3c", "4", "Parent", "PARENT" },
                    { "9a36cddc-ef6b-4984-9c0c-3e1c395f5076", "1", "Admin", "ADMIN" },
                    { "b5c6e34e-6780-4107-a529-fab3487b06db", "3", "Teacher", "TEACHER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TuitionFeeNotifications_ParentId",
                table: "TuitionFeeNotifications",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TuitionFeeNotifications");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "06c55c8a-e840-49af-a0b2-1820bb96c631");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f5a71f0-af09-42ab-a532-7d417f6d3a3c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a36cddc-ef6b-4984-9c0c-3e1c395f5076");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b5c6e34e-6780-4107-a529-fab3487b06db");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "369be6f5-ba37-4fb8-b2f0-fe33f58ccff9", "2", "Student", "STUDENT" },
                    { "5412561e-9a90-44b8-af53-6d51a3921702", "4", "Parent", "PARENT" },
                    { "61bb8ab6-0bd6-4920-b230-ced1554e997d", "3", "Teacher", "TEACHER" },
                    { "949e2998-36ed-42f0-a63f-fa0c019ffd0c", "1", "Admin", "ADMIN" }
                });
        }
    }
}
