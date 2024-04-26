using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagementSchool.Migrations
{
    public partial class FixedTotalPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TotalPoints_Students_StudentId",
                table: "TotalPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_TotalPoints_Students_StudentId1",
                table: "TotalPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TotalPoints",
                table: "TotalPoints");

            migrationBuilder.DropIndex(
                name: "IX_TotalPoints_StudentId1",
                table: "TotalPoints");

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

            migrationBuilder.RenameColumn(
                name: "StudentId1",
                table: "TotalPoints",
                newName: "TotalPointsId");

            migrationBuilder.AlterColumn<int>(
                name: "TotalPointsId",
                table: "TotalPoints",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TotalPoints",
                table: "TotalPoints",
                column: "TotalPointsId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0b8aff1e-bcf5-446b-af4c-5eb47db61631", "1", "Admin", "ADMIN" },
                    { "349ccb2b-f065-4ce0-829e-548eaf9ace8e", "2", "Student", "STUDENT" },
                    { "53309d46-b299-458b-ba60-fc479171f8bd", "4", "Parent", "PARENT" },
                    { "bcc681d3-0188-43c6-a0fe-7fe8ad6984fe", "3", "Teacher", "TEACHER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TotalPoints_StudentId",
                table: "TotalPoints",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TotalPoints_Students_StudentId",
                table: "TotalPoints",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TotalPoints_Students_StudentId",
                table: "TotalPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TotalPoints",
                table: "TotalPoints");

            migrationBuilder.DropIndex(
                name: "IX_TotalPoints_StudentId",
                table: "TotalPoints");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b8aff1e-bcf5-446b-af4c-5eb47db61631");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "349ccb2b-f065-4ce0-829e-548eaf9ace8e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "53309d46-b299-458b-ba60-fc479171f8bd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bcc681d3-0188-43c6-a0fe-7fe8ad6984fe");

            migrationBuilder.RenameColumn(
                name: "TotalPointsId",
                table: "TotalPoints",
                newName: "StudentId1");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId1",
                table: "TotalPoints",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TotalPoints",
                table: "TotalPoints",
                column: "StudentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TotalPoints_Students_StudentId",
                table: "TotalPoints",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TotalPoints_Students_StudentId1",
                table: "TotalPoints",
                column: "StudentId1",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
