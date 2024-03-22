using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementSchool.Migrations
{
    public partial class YearName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Parents_ParentId",
                table: "Students");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "43cceed5-0047-4bea-a699-ec1e1b62a5b2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "64688d76-85f9-4b0e-8124-8506ef8d3acd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "649f7f09-3747-4352-8432-fe27073691a6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bdb90264-3596-4199-b74d-f7e17b97ad88");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "SchoolYears",
                newName: "YearName");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "09f0aec2-a1b2-4738-b050-92447f18368e", "3", "Teacher", "TEACHER" },
                    { "55a9a466-653e-4bfb-a28c-6809b723a2d9", "2", "Student", "STUDENT" },
                    { "cf106257-23ca-4620-92e1-1899beb697c2", "4", "Parent", "PARENT" },
                    { "fb99aa9b-2b48-4156-80a8-7be3b07ad4d7", "1", "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Parents_ParentId",
                table: "Students",
                column: "ParentId",
                principalTable: "Parents",
                principalColumn: "ParentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Parents_ParentId",
                table: "Students");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "09f0aec2-a1b2-4738-b050-92447f18368e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55a9a466-653e-4bfb-a28c-6809b723a2d9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cf106257-23ca-4620-92e1-1899beb697c2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fb99aa9b-2b48-4156-80a8-7be3b07ad4d7");

            migrationBuilder.RenameColumn(
                name: "YearName",
                table: "SchoolYears",
                newName: "Name");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "43cceed5-0047-4bea-a699-ec1e1b62a5b2", "4", "Parent", "PARENT" },
                    { "64688d76-85f9-4b0e-8124-8506ef8d3acd", "2", "Student", "STUDENT" },
                    { "649f7f09-3747-4352-8432-fe27073691a6", "3", "Teacher", "TEACHER" },
                    { "bdb90264-3596-4199-b74d-f7e17b97ad88", "1", "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Parents_ParentId",
                table: "Students",
                column: "ParentId",
                principalTable: "Parents",
                principalColumn: "ParentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
