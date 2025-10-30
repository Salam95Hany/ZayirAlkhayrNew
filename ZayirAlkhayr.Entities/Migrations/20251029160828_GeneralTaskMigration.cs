using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZayirAlkhayr.Entities.Migrations
{
    /// <inheritdoc />
    public partial class GeneralTaskMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FamilyIncome_FamilyStatusId",
                schema: "admin",
                table: "FamilyIncome");

            migrationBuilder.DropIndex(
                name: "IX_FamilyExtraDetails_FamilyStatusId",
                schema: "admin",
                table: "FamilyExtraDetails");

            migrationBuilder.DropIndex(
                name: "IX_FamilyExpenses_FamilyStatusId",
                schema: "admin",
                table: "FamilyExpenses");

            migrationBuilder.EnsureSchema(
                name: "config");

            migrationBuilder.RenameColumn(
                name: "Task",
                schema: "admin",
                table: "GeneralTasks",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "SliderImages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "SliderImages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "Photos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "Photos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "Orphans",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "Orphans",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "GeneralTasks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "GeneralTasks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "admin",
                table: "GeneralTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                schema: "admin",
                table: "GeneralTasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                schema: "admin",
                table: "GeneralTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyStatus",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyStatus",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyPatientTypes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyPatientTypes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyNeedTypes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyNeedTypes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyNeedCategories",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyNeedCategories",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyNationalities",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyNationalities",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyCategories",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyCategories",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "Events",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "Events",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "BeneFactorTypes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "BeneFactorTypes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "BeneFactors",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "BeneFactors",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "BeneFactorNationalities",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "BeneFactorNationalities",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "BeneFactorDetails",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "BeneFactorDetails",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "Activities",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "Activities",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "AccountsImportMony",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "AccountsImportMony",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "AccountsExportMony",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "AccountsExportMony",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "config",
                columns: table => new
                {
                    ApplicationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "PagePermission",
                schema: "config",
                columns: table => new
                {
                    PagePermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanAdd = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CanExport = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagePermission", x => x.PagePermissionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SliderImages_InsertUser",
                schema: "web",
                table: "SliderImages",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_SliderImages_UpdateUser",
                schema: "web",
                table: "SliderImages",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_InsertUser",
                schema: "web",
                table: "Projects",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UpdateUser",
                schema: "web",
                table: "Projects",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_InsertUser",
                schema: "web",
                table: "Photos",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UpdateUser",
                schema: "web",
                table: "Photos",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_Orphans_InsertUser",
                schema: "admin",
                table: "Orphans",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_Orphans_UpdateUser",
                schema: "admin",
                table: "Orphans",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTasks_InsertUser",
                schema: "admin",
                table: "GeneralTasks",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTasks_UpdateUser",
                schema: "admin",
                table: "GeneralTasks",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyStatus_InsertUser",
                schema: "admin",
                table: "FamilyStatus",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyStatus_UpdateUser",
                schema: "admin",
                table: "FamilyStatus",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyPatientTypes_InsertUser",
                schema: "admin",
                table: "FamilyPatientTypes",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyPatientTypes_UpdateUser",
                schema: "admin",
                table: "FamilyPatientTypes",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyNeedTypes_InsertUser",
                schema: "admin",
                table: "FamilyNeedTypes",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyNeedTypes_UpdateUser",
                schema: "admin",
                table: "FamilyNeedTypes",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyNeedCategories_InsertUser",
                schema: "admin",
                table: "FamilyNeedCategories",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyNeedCategories_UpdateUser",
                schema: "admin",
                table: "FamilyNeedCategories",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyNationalities_InsertUser",
                schema: "admin",
                table: "FamilyNationalities",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyNationalities_UpdateUser",
                schema: "admin",
                table: "FamilyNationalities",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyIncome_FamilyStatusId",
                schema: "admin",
                table: "FamilyIncome",
                column: "FamilyStatusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamilyExtraDetails_FamilyStatusId",
                schema: "admin",
                table: "FamilyExtraDetails",
                column: "FamilyStatusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamilyExpenses_FamilyStatusId",
                schema: "admin",
                table: "FamilyExpenses",
                column: "FamilyStatusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamilyCategories_InsertUser",
                schema: "admin",
                table: "FamilyCategories",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyCategories_UpdateUser",
                schema: "admin",
                table: "FamilyCategories",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_Events_InsertUser",
                schema: "web",
                table: "Events",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UpdateUser",
                schema: "web",
                table: "Events",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_BeneFactorTypes_InsertUser",
                schema: "web",
                table: "BeneFactorTypes",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_BeneFactorTypes_UpdateUser",
                schema: "web",
                table: "BeneFactorTypes",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_BeneFactors_InsertUser",
                schema: "web",
                table: "BeneFactors",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_BeneFactors_UpdateUser",
                schema: "web",
                table: "BeneFactors",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_BeneFactorNationalities_InsertUser",
                schema: "web",
                table: "BeneFactorNationalities",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_BeneFactorNationalities_UpdateUser",
                schema: "web",
                table: "BeneFactorNationalities",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_BeneFactorDetails_InsertUser",
                schema: "web",
                table: "BeneFactorDetails",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_BeneFactorDetails_UpdateUser",
                schema: "web",
                table: "BeneFactorDetails",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_InsertUser",
                schema: "web",
                table: "Activities",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_UpdateUser",
                schema: "web",
                table: "Activities",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsImportMony_InsertUser",
                schema: "admin",
                table: "AccountsImportMony",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsImportMony_UpdateUser",
                schema: "admin",
                table: "AccountsImportMony",
                column: "UpdateUser");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsExportMony_InsertUser",
                schema: "admin",
                table: "AccountsExportMony",
                column: "InsertUser");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsExportMony_UpdateUser",
                schema: "admin",
                table: "AccountsExportMony",
                column: "UpdateUser");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsExportMony_AspNetUsers_InsertUser",
                schema: "admin",
                table: "AccountsExportMony",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsExportMony_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "AccountsExportMony",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsImportMony_AspNetUsers_InsertUser",
                schema: "admin",
                table: "AccountsImportMony",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsImportMony_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "AccountsImportMony",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_AspNetUsers_InsertUser",
                schema: "web",
                table: "Activities",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_AspNetUsers_UpdateUser",
                schema: "web",
                table: "Activities",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BeneFactorDetails_AspNetUsers_InsertUser",
                schema: "web",
                table: "BeneFactorDetails",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BeneFactorDetails_AspNetUsers_UpdateUser",
                schema: "web",
                table: "BeneFactorDetails",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BeneFactorNationalities_AspNetUsers_InsertUser",
                schema: "web",
                table: "BeneFactorNationalities",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BeneFactorNationalities_AspNetUsers_UpdateUser",
                schema: "web",
                table: "BeneFactorNationalities",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BeneFactors_AspNetUsers_InsertUser",
                schema: "web",
                table: "BeneFactors",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BeneFactors_AspNetUsers_UpdateUser",
                schema: "web",
                table: "BeneFactors",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BeneFactorTypes_AspNetUsers_InsertUser",
                schema: "web",
                table: "BeneFactorTypes",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BeneFactorTypes_AspNetUsers_UpdateUser",
                schema: "web",
                table: "BeneFactorTypes",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_InsertUser",
                schema: "web",
                table: "Events",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_UpdateUser",
                schema: "web",
                table: "Events",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyCategories_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyCategories",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyCategories_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyCategories",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyNationalities_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyNationalities",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyNationalities_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyNationalities",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyNeedCategories_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyNeedCategories",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyNeedCategories_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyNeedCategories",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyNeedTypes_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyNeedTypes",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyNeedTypes_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyNeedTypes",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyPatientTypes_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyPatientTypes",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyPatientTypes_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyPatientTypes",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyStatus_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyStatus",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyStatus_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyStatus",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralTasks_AspNetUsers_InsertUser",
                schema: "admin",
                table: "GeneralTasks",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralTasks_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "GeneralTasks",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orphans_AspNetUsers_InsertUser",
                schema: "admin",
                table: "Orphans",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orphans_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "Orphans",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AspNetUsers_InsertUser",
                schema: "web",
                table: "Photos",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_AspNetUsers_UpdateUser",
                schema: "web",
                table: "Photos",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_InsertUser",
                schema: "web",
                table: "Projects",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_UpdateUser",
                schema: "web",
                table: "Projects",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SliderImages_AspNetUsers_InsertUser",
                schema: "web",
                table: "SliderImages",
                column: "InsertUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SliderImages_AspNetUsers_UpdateUser",
                schema: "web",
                table: "SliderImages",
                column: "UpdateUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountsExportMony_AspNetUsers_InsertUser",
                schema: "admin",
                table: "AccountsExportMony");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountsExportMony_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "AccountsExportMony");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountsImportMony_AspNetUsers_InsertUser",
                schema: "admin",
                table: "AccountsImportMony");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountsImportMony_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "AccountsImportMony");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_AspNetUsers_InsertUser",
                schema: "web",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_AspNetUsers_UpdateUser",
                schema: "web",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_BeneFactorDetails_AspNetUsers_InsertUser",
                schema: "web",
                table: "BeneFactorDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BeneFactorDetails_AspNetUsers_UpdateUser",
                schema: "web",
                table: "BeneFactorDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BeneFactorNationalities_AspNetUsers_InsertUser",
                schema: "web",
                table: "BeneFactorNationalities");

            migrationBuilder.DropForeignKey(
                name: "FK_BeneFactorNationalities_AspNetUsers_UpdateUser",
                schema: "web",
                table: "BeneFactorNationalities");

            migrationBuilder.DropForeignKey(
                name: "FK_BeneFactors_AspNetUsers_InsertUser",
                schema: "web",
                table: "BeneFactors");

            migrationBuilder.DropForeignKey(
                name: "FK_BeneFactors_AspNetUsers_UpdateUser",
                schema: "web",
                table: "BeneFactors");

            migrationBuilder.DropForeignKey(
                name: "FK_BeneFactorTypes_AspNetUsers_InsertUser",
                schema: "web",
                table: "BeneFactorTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_BeneFactorTypes_AspNetUsers_UpdateUser",
                schema: "web",
                table: "BeneFactorTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_InsertUser",
                schema: "web",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_UpdateUser",
                schema: "web",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyCategories_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyCategories_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyNationalities_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyNationalities");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyNationalities_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyNationalities");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyNeedCategories_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyNeedCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyNeedCategories_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyNeedCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyNeedTypes_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyNeedTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyNeedTypes_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyNeedTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyPatientTypes_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyPatientTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyPatientTypes_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyPatientTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyStatus_AspNetUsers_InsertUser",
                schema: "admin",
                table: "FamilyStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_FamilyStatus_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "FamilyStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralTasks_AspNetUsers_InsertUser",
                schema: "admin",
                table: "GeneralTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneralTasks_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "GeneralTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Orphans_AspNetUsers_InsertUser",
                schema: "admin",
                table: "Orphans");

            migrationBuilder.DropForeignKey(
                name: "FK_Orphans_AspNetUsers_UpdateUser",
                schema: "admin",
                table: "Orphans");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AspNetUsers_InsertUser",
                schema: "web",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_AspNetUsers_UpdateUser",
                schema: "web",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_InsertUser",
                schema: "web",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_UpdateUser",
                schema: "web",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_SliderImages_AspNetUsers_InsertUser",
                schema: "web",
                table: "SliderImages");

            migrationBuilder.DropForeignKey(
                name: "FK_SliderImages_AspNetUsers_UpdateUser",
                schema: "web",
                table: "SliderImages");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "config");

            migrationBuilder.DropTable(
                name: "PagePermission",
                schema: "config");

            migrationBuilder.DropIndex(
                name: "IX_SliderImages_InsertUser",
                schema: "web",
                table: "SliderImages");

            migrationBuilder.DropIndex(
                name: "IX_SliderImages_UpdateUser",
                schema: "web",
                table: "SliderImages");

            migrationBuilder.DropIndex(
                name: "IX_Projects_InsertUser",
                schema: "web",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UpdateUser",
                schema: "web",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Photos_InsertUser",
                schema: "web",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_UpdateUser",
                schema: "web",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Orphans_InsertUser",
                schema: "admin",
                table: "Orphans");

            migrationBuilder.DropIndex(
                name: "IX_Orphans_UpdateUser",
                schema: "admin",
                table: "Orphans");

            migrationBuilder.DropIndex(
                name: "IX_GeneralTasks_InsertUser",
                schema: "admin",
                table: "GeneralTasks");

            migrationBuilder.DropIndex(
                name: "IX_GeneralTasks_UpdateUser",
                schema: "admin",
                table: "GeneralTasks");

            migrationBuilder.DropIndex(
                name: "IX_FamilyStatus_InsertUser",
                schema: "admin",
                table: "FamilyStatus");

            migrationBuilder.DropIndex(
                name: "IX_FamilyStatus_UpdateUser",
                schema: "admin",
                table: "FamilyStatus");

            migrationBuilder.DropIndex(
                name: "IX_FamilyPatientTypes_InsertUser",
                schema: "admin",
                table: "FamilyPatientTypes");

            migrationBuilder.DropIndex(
                name: "IX_FamilyPatientTypes_UpdateUser",
                schema: "admin",
                table: "FamilyPatientTypes");

            migrationBuilder.DropIndex(
                name: "IX_FamilyNeedTypes_InsertUser",
                schema: "admin",
                table: "FamilyNeedTypes");

            migrationBuilder.DropIndex(
                name: "IX_FamilyNeedTypes_UpdateUser",
                schema: "admin",
                table: "FamilyNeedTypes");

            migrationBuilder.DropIndex(
                name: "IX_FamilyNeedCategories_InsertUser",
                schema: "admin",
                table: "FamilyNeedCategories");

            migrationBuilder.DropIndex(
                name: "IX_FamilyNeedCategories_UpdateUser",
                schema: "admin",
                table: "FamilyNeedCategories");

            migrationBuilder.DropIndex(
                name: "IX_FamilyNationalities_InsertUser",
                schema: "admin",
                table: "FamilyNationalities");

            migrationBuilder.DropIndex(
                name: "IX_FamilyNationalities_UpdateUser",
                schema: "admin",
                table: "FamilyNationalities");

            migrationBuilder.DropIndex(
                name: "IX_FamilyIncome_FamilyStatusId",
                schema: "admin",
                table: "FamilyIncome");

            migrationBuilder.DropIndex(
                name: "IX_FamilyExtraDetails_FamilyStatusId",
                schema: "admin",
                table: "FamilyExtraDetails");

            migrationBuilder.DropIndex(
                name: "IX_FamilyExpenses_FamilyStatusId",
                schema: "admin",
                table: "FamilyExpenses");

            migrationBuilder.DropIndex(
                name: "IX_FamilyCategories_InsertUser",
                schema: "admin",
                table: "FamilyCategories");

            migrationBuilder.DropIndex(
                name: "IX_FamilyCategories_UpdateUser",
                schema: "admin",
                table: "FamilyCategories");

            migrationBuilder.DropIndex(
                name: "IX_Events_InsertUser",
                schema: "web",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_UpdateUser",
                schema: "web",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_BeneFactorTypes_InsertUser",
                schema: "web",
                table: "BeneFactorTypes");

            migrationBuilder.DropIndex(
                name: "IX_BeneFactorTypes_UpdateUser",
                schema: "web",
                table: "BeneFactorTypes");

            migrationBuilder.DropIndex(
                name: "IX_BeneFactors_InsertUser",
                schema: "web",
                table: "BeneFactors");

            migrationBuilder.DropIndex(
                name: "IX_BeneFactors_UpdateUser",
                schema: "web",
                table: "BeneFactors");

            migrationBuilder.DropIndex(
                name: "IX_BeneFactorNationalities_InsertUser",
                schema: "web",
                table: "BeneFactorNationalities");

            migrationBuilder.DropIndex(
                name: "IX_BeneFactorNationalities_UpdateUser",
                schema: "web",
                table: "BeneFactorNationalities");

            migrationBuilder.DropIndex(
                name: "IX_BeneFactorDetails_InsertUser",
                schema: "web",
                table: "BeneFactorDetails");

            migrationBuilder.DropIndex(
                name: "IX_BeneFactorDetails_UpdateUser",
                schema: "web",
                table: "BeneFactorDetails");

            migrationBuilder.DropIndex(
                name: "IX_Activities_InsertUser",
                schema: "web",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_UpdateUser",
                schema: "web",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_AccountsImportMony_InsertUser",
                schema: "admin",
                table: "AccountsImportMony");

            migrationBuilder.DropIndex(
                name: "IX_AccountsImportMony_UpdateUser",
                schema: "admin",
                table: "AccountsImportMony");

            migrationBuilder.DropIndex(
                name: "IX_AccountsExportMony_InsertUser",
                schema: "admin",
                table: "AccountsExportMony");

            migrationBuilder.DropIndex(
                name: "IX_AccountsExportMony_UpdateUser",
                schema: "admin",
                table: "AccountsExportMony");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "admin",
                table: "GeneralTasks");

            migrationBuilder.DropColumn(
                name: "DueDate",
                schema: "admin",
                table: "GeneralTasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                schema: "admin",
                table: "GeneralTasks");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "admin",
                table: "GeneralTasks",
                newName: "Task");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "SliderImages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "SliderImages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "Orphans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "Orphans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "GeneralTasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "GeneralTasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyStatus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyStatus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyPatientTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyPatientTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyNeedTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyNeedTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyNeedCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyNeedCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyNationalities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyNationalities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "FamilyCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "FamilyCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "BeneFactorTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "BeneFactorTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "BeneFactors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "BeneFactors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "BeneFactorNationalities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "BeneFactorNationalities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "BeneFactorDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "BeneFactorDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "web",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "web",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "AccountsImportMony",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "AccountsImportMony",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUser",
                schema: "admin",
                table: "AccountsExportMony",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsertUser",
                schema: "admin",
                table: "AccountsExportMony",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamilyIncome_FamilyStatusId",
                schema: "admin",
                table: "FamilyIncome",
                column: "FamilyStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyExtraDetails_FamilyStatusId",
                schema: "admin",
                table: "FamilyExtraDetails",
                column: "FamilyStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyExpenses_FamilyStatusId",
                schema: "admin",
                table: "FamilyExpenses",
                column: "FamilyStatusId");
        }
    }
}
