using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementSchool.Migrations
{
    public partial class Subjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0f7c170c-20f2-4e12-92cd-1418fe53b162");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "28507071-8b3f-4143-a9a1-918c692ae472");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "628f0aca-3fe8-4125-a051-94e8d4680780");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b67ab37e-0124-4f81-98d6-90fceee79591");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5a1d1d9a-fd9b-489e-a49f-2b0dff905764", "2", "Student", "STUDENT" },
                    { "b8da9d95-2122-4496-b3e9-1ae305e17970", "1", "Admin", "ADMIN" },
                    { "d412e280-f9a1-4eca-afad-24e40672dc7b", "3", "Teacher", "TEACHER" },
                    { "d468df2c-ef8c-418d-bc50-065ebd075e18", "4", "Parent", "PARENT" }
                });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 1,
                column: "SubjectName",
                value: "Math");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 2,
                column: "SubjectName",
                value: "Literature");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 3,
                column: "SubjectName",
                value: "English");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 4,
                column: "SubjectName",
                value: "Physics");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 5,
                column: "SubjectName",
                value: "Chemistry");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 6,
                column: "SubjectName",
                value: "Biology");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 7,
                column: "SubjectName",
                value: "History");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 8,
                column: "SubjectName",
                value: "Geography");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 9,
                column: "SubjectName",
                value: "Civics");

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "SubjectId", "StudentId", "SubjectName" },
                values: new object[] { 10, null, "Computer Science" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a1d1d9a-fd9b-489e-a49f-2b0dff905764");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b8da9d95-2122-4496-b3e9-1ae305e17970");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d412e280-f9a1-4eca-afad-24e40672dc7b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d468df2c-ef8c-418d-bc50-065ebd075e18");

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 10);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0f7c170c-20f2-4e12-92cd-1418fe53b162", "4", "Parent", "PARENT" },
                    { "28507071-8b3f-4143-a9a1-918c692ae472", "1", "Admin", "ADMIN" },
                    { "628f0aca-3fe8-4125-a051-94e8d4680780", "3", "Teacher", "TEACHER" },
                    { "b67ab37e-0124-4f81-98d6-90fceee79591", "2", "Student", "STUDENT" }
                });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 1,
                column: "SubjectName",
                value: "Mathematics");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 2,
                column: "SubjectName",
                value: "Science");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 3,
                column: "SubjectName",
                value: "History");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 4,
                column: "SubjectName",
                value: "Geography");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 5,
                column: "SubjectName",
                value: "Physical Education");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 6,
                column: "SubjectName",
                value: "Art");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 7,
                column: "SubjectName",
                value: "Music");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 8,
                column: "SubjectName",
                value: "English");

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "SubjectId",
                keyValue: 9,
                column: "SubjectName",
                value: "Foreign Languages");
        }
    }
}
