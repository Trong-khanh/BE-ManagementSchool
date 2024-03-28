using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementSchool.Migrations
{
    public partial class phonenumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "066042df-7708-4262-b8f2-674324cba854");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "933f1592-b005-4425-b9e1-4781c8115fcb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "daea4cbf-a02a-43b0-ab7e-3d9b0eadc1cc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ee7e52e6-e270-4ed5-b8b0-3a2dba58a285");

            migrationBuilder.AddColumn<string>(
                name: "ParentPhoneNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ParentPhoneNumber",
                table: "Students");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "066042df-7708-4262-b8f2-674324cba854", "2", "Student", "STUDENT" },
                    { "933f1592-b005-4425-b9e1-4781c8115fcb", "4", "Parent", "PARENT" },
                    { "daea4cbf-a02a-43b0-ab7e-3d9b0eadc1cc", "1", "Admin", "ADMIN" },
                    { "ee7e52e6-e270-4ed5-b8b0-3a2dba58a285", "3", "Teacher", "TEACHER" }
                });
        }
    }
}
