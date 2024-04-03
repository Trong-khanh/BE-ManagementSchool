using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementSchool.Migrations
{
    public partial class score : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "76ecaec2-d2fa-454b-9795-0ed8b436cf3b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a9ead909-0a56-4dc8-b42d-b3e351c5e7c3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dee47561-9c6f-409e-b2d7-1dcc0fbdd21e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fc17f065-ce8a-4077-91d0-095f2f777a45");

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    ScoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    SemesterName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.ScoreId);
                    table.ForeignKey(
                        name: "FK_Scores_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scores_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2ef7a951-26e6-4c0e-85d0-977eeeca1885", "2", "Student", "STUDENT" },
                    { "5802ffe4-466d-4246-bcb6-7199987c79ce", "1", "Admin", "ADMIN" },
                    { "5c818d5d-5bf7-4436-b669-14070c31861a", "4", "Parent", "PARENT" },
                    { "cea5a0dc-b6d2-436f-98b6-c07c7fb4a46c", "3", "Teacher", "TEACHER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scores_StudentId",
                table: "Scores",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_SubjectId",
                table: "Scores",
                column: "SubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2ef7a951-26e6-4c0e-85d0-977eeeca1885");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5802ffe4-466d-4246-bcb6-7199987c79ce");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5c818d5d-5bf7-4436-b669-14070c31861a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cea5a0dc-b6d2-436f-98b6-c07c7fb4a46c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "76ecaec2-d2fa-454b-9795-0ed8b436cf3b", "1", "Admin", "ADMIN" },
                    { "a9ead909-0a56-4dc8-b42d-b3e351c5e7c3", "4", "Parent", "PARENT" },
                    { "dee47561-9c6f-409e-b2d7-1dcc0fbdd21e", "2", "Student", "STUDENT" },
                    { "fc17f065-ce8a-4077-91d0-095f2f777a45", "3", "Teacher", "TEACHER" }
                });
        }
    }
}
