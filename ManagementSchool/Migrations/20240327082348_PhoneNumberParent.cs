using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementSchool.Migrations
{
    public partial class PhoneNumberParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "ParentPhoneNumber",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ParentPhoneNumber",
                table: "Parents");

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
        }
    }
}
