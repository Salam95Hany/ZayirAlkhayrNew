using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZayirAlkhayr.Entities.Migrations
{
    /// <inheritdoc />
    public partial class StudentMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "school");

            migrationBuilder.CreateTable(
                name: "AcademicStages",
                schema: "school",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    InsertUser = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicStages_AspNetUsers_InsertUser",
                        column: x => x.InsertUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AcademicStages_AspNetUsers_UpdateUser",
                        column: x => x.UpdateUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DiscountTypes",
                schema: "school",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsertUser = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountTypes_AspNetUsers_InsertUser",
                        column: x => x.InsertUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DiscountTypes_AspNetUsers_UpdateUser",
                        column: x => x.UpdateUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Parents",
                schema: "school",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelegramCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentNationalities",
                schema: "school",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsertUser = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentNationalities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentNationalities_AspNetUsers_InsertUser",
                        column: x => x.InsertUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentNationalities_AspNetUsers_UpdateUser",
                        column: x => x.UpdateUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                schema: "school",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicStageId = table.Column<int>(type: "int", nullable: false),
                    NationalityId = table.Column<int>(type: "int", nullable: false),
                    StudentStatusId = table.Column<int>(type: "int", nullable: false),
                    DiscountTypeId = table.Column<int>(type: "int", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    BirthDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GovernmentSchool = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    StudyPeriod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHaveHealthCondition = table.Column<bool>(type: "bit", nullable: false),
                    HealthConditionNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyAmount = table.Column<double>(type: "float", nullable: false),
                    StudentStatusReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderAmongChildren = table.Column<int>(type: "int", nullable: false),
                    DiscountReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountAmount = table.Column<double>(type: "float", nullable: true),
                    ChildrenCount = table.Column<int>(type: "int", nullable: false),
                    InsertUser = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_InsertUser",
                        column: x => x.InsertUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_UpdateUser",
                        column: x => x.UpdateUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicStages_InsertUser",
                schema: "school",
                table: "AcademicStages",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicStages_UpdateUser",
                schema: "school",
                table: "AcademicStages",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountTypes_InsertUser",
                schema: "school",
                table: "DiscountTypes",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountTypes_UpdateUser",
                schema: "school",
                table: "DiscountTypes",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNationalities_InsertUser",
                schema: "school",
                table: "StudentNationalities",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNationalities_UpdateUser",
                schema: "school",
                table: "StudentNationalities",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_Students_InsertUser",
                schema: "school",
                table: "Students",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UpdateUser",
                schema: "school",
                table: "Students",
                column: "UpdateUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicStages",
                schema: "school");

            migrationBuilder.DropTable(
                name: "DiscountTypes",
                schema: "school");

            migrationBuilder.DropTable(
                name: "Parents",
                schema: "school");

            migrationBuilder.DropTable(
                name: "StudentNationalities",
                schema: "school");

            migrationBuilder.DropTable(
                name: "Students",
                schema: "school");
        }
    }
}
