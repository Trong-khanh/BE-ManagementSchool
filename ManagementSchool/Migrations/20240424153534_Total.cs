using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementSchool.Migrations
{
    public partial class Total : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "465fde1f-cc79-444d-9faa-47b8882050db");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d0cff29e-b449-4b09-bc65-fccd3f1d23da");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e63a5d6d-d197-4fc4-91ab-a2afeac21822");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f9586ce8-1b38-49e4-9297-eb4e0a47f81b");

            migrationBuilder.CreateTable(
                name: "TotalPoints",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TotalSemester1 = table.Column<double>(type: "float", nullable: false),
                    TotalSemester2 = table.Column<double>(type: "float", nullable: false),
                    TotalYear = table.Column<double>(type: "float", nullable: false),
                    StudentId1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TotalPoints", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_TotalPoints_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TotalPoints_Students_StudentId1",
                        column: x => x.StudentId1,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "31725a3b-3d56-4c8d-98e3-83975dce4770", "1", "Admin", "ADMIN" },
                    { "8e0617ee-1fea-495a-9627-c5d11edc32fd", "3", "Teacher", "TEACHER" },
                    { "920a44bd-6efb-438e-aa53-47a578c5184b", "4", "Parent", "PARENT" },
                    { "fa0b083d-7339-4081-8846-7b321b2a6670", "2", "Student", "STUDENT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TotalPoints_StudentId1",
                table: "TotalPoints",
                column: "StudentId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TotalPoints");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "31725a3b-3d56-4c8d-98e3-83975dce4770");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8e0617ee-1fea-495a-9627-c5d11edc32fd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "920a44bd-6efb-438e-aa53-47a578c5184b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fa0b083d-7339-4081-8846-7b321b2a6670");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "465fde1f-cc79-444d-9faa-47b8882050db", "4", "Parent", "PARENT" },
                    { "d0cff29e-b449-4b09-bc65-fccd3f1d23da", "3", "Teacher", "TEACHER" },
                    { "e63a5d6d-d197-4fc4-91ab-a2afeac21822", "2", "Student", "STUDENT" },
                    { "f9586ce8-1b38-49e4-9297-eb4e0a47f81b", "1", "Admin", "ADMIN" }
                });
        }
    }
}
