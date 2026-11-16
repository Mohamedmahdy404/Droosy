using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Droosy_DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemainingFeaturesAndVisitorIsolation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivationCodes_Courses_CourseId",
                table: "ActivationCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivationCodes_Users_RedeemedByStudentId",
                table: "ActivationCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_ClassSessions_SessionId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Users_StudentId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_ActorId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassSessions_StudyGroups_GroupId",
                table: "ClassSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AcademicLevels_LevelId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_TeacherId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseUnits_Courses_CourseId",
                table: "CourseUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupStudents_StudyGroups_GroupId",
                table: "GroupStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupStudents_Users_StudentId",
                table: "GroupStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonProgresses_Lessons_LessonId",
                table: "LessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonProgresses_Users_StudentId",
                table: "LessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_CourseUnits_UnitId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentStudents_Users_ParentId",
                table: "ParentStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentStudents_Users_StudentId",
                table: "ParentStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Subscriptions_SubscriptionId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_RecordedByUserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyGroups_Courses_CourseId",
                table: "StudyGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Courses_CourseId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_StudentId",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_CourseId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_StudentId",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyGroups",
                table: "StudyGroups");

            migrationBuilder.DropIndex(
                name: "IX_StudyGroups_CourseId",
                table: "StudyGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformSettings",
                table: "PlatformSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_RecordedByUserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParentStudents",
                table: "ParentStudents");

            migrationBuilder.DropIndex(
                name: "IX_ParentStudents_StudentId",
                table: "ParentStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_UnitId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonProgresses",
                table: "LessonProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LessonProgresses_LessonId",
                table: "LessonProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LessonProgresses_StudentId_LessonId",
                table: "LessonProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupStudents",
                table: "GroupStudents");

            migrationBuilder.DropIndex(
                name: "IX_GroupStudents_StudentId",
                table: "GroupStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DemoStates",
                table: "DemoStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseUnits",
                table: "CourseUnits");

            migrationBuilder.DropIndex(
                name: "IX_CourseUnits_CourseId",
                table: "CourseUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_LevelId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassSessions",
                table: "ClassSessions");

            migrationBuilder.DropIndex(
                name: "IX_ClassSessions_GroupId",
                table: "ClassSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_ActorId",
                table: "AuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_SessionId_StudentId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivationCodes",
                table: "ActivationCodes");

            migrationBuilder.DropIndex(
                name: "IX_ActivationCodes_Code",
                table: "ActivationCodes");

            migrationBuilder.DropIndex(
                name: "IX_ActivationCodes_CourseId",
                table: "ActivationCodes");

            migrationBuilder.DropIndex(
                name: "IX_ActivationCodes_RedeemedByStudentId",
                table: "ActivationCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicLevels",
                table: "AcademicLevels");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Users",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "Users",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Subscriptions",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "Subscriptions",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Subscriptions",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "Subscriptions",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "StudyGroups",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StudyGroups",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "StudyGroups",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PlatformSettings",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "PlatformSettings",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "SubscriptionId",
                table: "Payments",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "RecordedByUserId",
                table: "Payments",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Payments",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "Payments",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "ParentStudents",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "ParentStudents",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "ParentStudents",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "UnitId",
                table: "Lessons",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Lessons",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "Lessons",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "LessonProgresses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LessonId",
                table: "LessonProgresses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "LessonProgresses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "LessonProgresses",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "GroupStudents",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "GroupId",
                table: "GroupStudents",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "GroupStudents",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "DemoStates",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "DemoStates",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AddColumn<Guid>(
                name: "RuntimeId",
                table: "DemoStates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "CourseUnits",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CourseUnits",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "CourseUnits",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "Courses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LevelId",
                table: "Courses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Courses",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "Courses",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "GroupId",
                table: "ClassSessions",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ClassSessions",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "ClassSessions",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "EntityId",
                table: "AuditLogs",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ActorId",
                table: "AuditLogs",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AuditLogs",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "AuditLogs",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Attendances",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "Attendances",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Attendances",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "Attendances",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "RedeemedByStudentId",
                table: "ActivationCodes",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "ActivationCodes",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ActivationCodes",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "ActivationCodes",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AcademicLevels",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VisitorId",
                table: "AcademicLevels",
                type: "varchar(64)",
                unicode: false,
                maxLength: 64,
                nullable: false,
                defaultValue: "template");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyGroups",
                table: "StudyGroups",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformSettings",
                table: "PlatformSettings",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParentStudents",
                table: "ParentStudents",
                columns: new[] { "VisitorId", "ParentId", "StudentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonProgresses",
                table: "LessonProgresses",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupStudents",
                table: "GroupStudents",
                columns: new[] { "VisitorId", "GroupId", "StudentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DemoStates",
                table: "DemoStates",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseUnits",
                table: "CourseUnits",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassSessions",
                table: "ClassSessions",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivationCodes",
                table: "ActivationCodes",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicLevels",
                table: "AcademicLevels",
                columns: new[] { "VisitorId", "Id" });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    GroupId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_Announcements_StudyGroups_VisitorId_GroupId",
                        columns: x => new { x.VisitorId, x.GroupId },
                        principalTable: "StudyGroups",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_Assignments_Courses_VisitorId_CourseId",
                        columns: x => new { x.VisitorId, x.CourseId },
                        principalTable: "Courses",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    TeacherId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    OtherUserId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ContextType = table.Column<int>(type: "int", nullable: false),
                    ContextId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_Conversations_Users_VisitorId_OtherUserId",
                        columns: x => new { x.VisitorId, x.OtherUserId },
                        principalTable: "Users",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_Users_VisitorId_StudentId",
                        columns: x => new { x.VisitorId, x.StudentId },
                        principalTable: "Users",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_Users_VisitorId_TeacherId",
                        columns: x => new { x.VisitorId, x.TeacherId },
                        principalTable: "Users",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpensAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosesAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_Exams_Courses_VisitorId_CourseId",
                        columns: x => new { x.VisitorId, x.CourseId },
                        principalTable: "Courses",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_Notifications_Users_VisitorId_UserId",
                        columns: x => new { x.VisitorId, x.UserId },
                        principalTable: "Users",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentSubmissions",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    AssignmentId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GradedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedScore = table.Column<int>(type: "int", nullable: true),
                    PublishedFeedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnReason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentSubmissions", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_AssignmentSubmissions_Assignments_VisitorId_AssignmentId",
                        columns: x => new { x.VisitorId, x.AssignmentId },
                        principalTable: "Assignments",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignmentSubmissions_Users_VisitorId_StudentId",
                        columns: x => new { x.VisitorId, x.StudentId },
                        principalTable: "Users",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ConversationId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_VisitorId_ConversationId",
                        columns: x => new { x.VisitorId, x.ConversationId },
                        principalTable: "Conversations",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_VisitorId_SenderId",
                        columns: x => new { x.VisitorId, x.SenderId },
                        principalTable: "Users",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamAttempts",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ExamId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeadlineAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamAttempts", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_ExamAttempts_Exams_VisitorId_ExamId",
                        columns: x => new { x.VisitorId, x.ExamId },
                        principalTable: "Exams",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamAttempts_Users_VisitorId_StudentId",
                        columns: x => new { x.VisitorId, x.StudentId },
                        principalTable: "Users",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestions",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ExamId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestions", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_ExamQuestions_Exams_VisitorId_ExamId",
                        columns: x => new { x.VisitorId, x.ExamId },
                        principalTable: "Exams",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamChoices",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamChoices", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_ExamChoices_ExamQuestions_VisitorId_QuestionId",
                        columns: x => new { x.VisitorId, x.QuestionId },
                        principalTable: "ExamQuestions",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamAnswers",
                columns: table => new
                {
                    VisitorId = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false, defaultValue: "template"),
                    Id = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    AttemptId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    ChoiceId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamAnswers", x => new { x.VisitorId, x.Id });
                    table.ForeignKey(
                        name: "FK_ExamAnswers_ExamAttempts_VisitorId_AttemptId",
                        columns: x => new { x.VisitorId, x.AttemptId },
                        principalTable: "ExamAttempts",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamAnswers_ExamChoices_VisitorId_ChoiceId",
                        columns: x => new { x.VisitorId, x.ChoiceId },
                        principalTable: "ExamChoices",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamAnswers_ExamQuestions_VisitorId_QuestionId",
                        columns: x => new { x.VisitorId, x.QuestionId },
                        principalTable: "ExamQuestions",
                        principalColumns: new[] { "VisitorId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_VisitorId_CourseId",
                table: "Subscriptions",
                columns: new[] { "VisitorId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_VisitorId_StudentId",
                table: "Subscriptions",
                columns: new[] { "VisitorId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudyGroups_VisitorId_CourseId",
                table: "StudyGroups",
                columns: new[] { "VisitorId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_VisitorId_RecordedByUserId",
                table: "Payments",
                columns: new[] { "VisitorId", "RecordedByUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_VisitorId_SubscriptionId",
                table: "Payments",
                columns: new[] { "VisitorId", "SubscriptionId" });

            migrationBuilder.CreateIndex(
                name: "IX_ParentStudents_VisitorId_StudentId",
                table: "ParentStudents",
                columns: new[] { "VisitorId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_VisitorId_UnitId",
                table: "Lessons",
                columns: new[] { "VisitorId", "UnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgresses_VisitorId_LessonId",
                table: "LessonProgresses",
                columns: new[] { "VisitorId", "LessonId" });

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgresses_VisitorId_StudentId",
                table: "LessonProgresses",
                columns: new[] { "VisitorId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgresses_VisitorId_StudentId_LessonId",
                table: "LessonProgresses",
                columns: new[] { "VisitorId", "StudentId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupStudents_VisitorId_StudentId",
                table: "GroupStudents",
                columns: new[] { "VisitorId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseUnits_VisitorId_CourseId",
                table: "CourseUnits",
                columns: new[] { "VisitorId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_VisitorId_LevelId",
                table: "Courses",
                columns: new[] { "VisitorId", "LevelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_VisitorId_TeacherId",
                table: "Courses",
                columns: new[] { "VisitorId", "TeacherId" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_VisitorId_GroupId",
                table: "ClassSessions",
                columns: new[] { "VisitorId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_VisitorId_ActorId",
                table: "AuditLogs",
                columns: new[] { "VisitorId", "ActorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_VisitorId_SessionId",
                table: "Attendances",
                columns: new[] { "VisitorId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_VisitorId_SessionId_StudentId",
                table: "Attendances",
                columns: new[] { "VisitorId", "SessionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_VisitorId_StudentId",
                table: "Attendances",
                columns: new[] { "VisitorId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCodes_VisitorId_Code",
                table: "ActivationCodes",
                columns: new[] { "VisitorId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCodes_VisitorId_CourseId",
                table: "ActivationCodes",
                columns: new[] { "VisitorId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCodes_VisitorId_RedeemedByStudentId",
                table: "ActivationCodes",
                columns: new[] { "VisitorId", "RedeemedByStudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_VisitorId_GroupId",
                table: "Announcements",
                columns: new[] { "VisitorId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_VisitorId_CourseId",
                table: "Assignments",
                columns: new[] { "VisitorId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_VisitorId_AssignmentId",
                table: "AssignmentSubmissions",
                columns: new[] { "VisitorId", "AssignmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_VisitorId_AssignmentId_StudentId",
                table: "AssignmentSubmissions",
                columns: new[] { "VisitorId", "AssignmentId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_VisitorId_StudentId",
                table: "AssignmentSubmissions",
                columns: new[] { "VisitorId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_VisitorId_OtherUserId",
                table: "Conversations",
                columns: new[] { "VisitorId", "OtherUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_VisitorId_StudentId",
                table: "Conversations",
                columns: new[] { "VisitorId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_VisitorId_TeacherId",
                table: "Conversations",
                columns: new[] { "VisitorId", "TeacherId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_VisitorId_AttemptId",
                table: "ExamAnswers",
                columns: new[] { "VisitorId", "AttemptId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_VisitorId_AttemptId_QuestionId",
                table: "ExamAnswers",
                columns: new[] { "VisitorId", "AttemptId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_VisitorId_ChoiceId",
                table: "ExamAnswers",
                columns: new[] { "VisitorId", "ChoiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_VisitorId_QuestionId",
                table: "ExamAnswers",
                columns: new[] { "VisitorId", "QuestionId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempts_VisitorId_ExamId",
                table: "ExamAttempts",
                columns: new[] { "VisitorId", "ExamId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempts_VisitorId_ExamId_StudentId",
                table: "ExamAttempts",
                columns: new[] { "VisitorId", "ExamId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempts_VisitorId_StudentId",
                table: "ExamAttempts",
                columns: new[] { "VisitorId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamChoices_VisitorId_QuestionId",
                table: "ExamChoices",
                columns: new[] { "VisitorId", "QuestionId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_VisitorId_ExamId",
                table: "ExamQuestions",
                columns: new[] { "VisitorId", "ExamId" });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_VisitorId_CourseId",
                table: "Exams",
                columns: new[] { "VisitorId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_VisitorId_ConversationId",
                table: "Messages",
                columns: new[] { "VisitorId", "ConversationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_VisitorId_SenderId",
                table: "Messages",
                columns: new[] { "VisitorId", "SenderId" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_VisitorId_UserId",
                table: "Notifications",
                columns: new[] { "VisitorId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_VisitorId_UserId_EventKey",
                table: "Notifications",
                columns: new[] { "VisitorId", "UserId", "EventKey" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationCodes_Courses_VisitorId_CourseId",
                table: "ActivationCodes",
                columns: new[] { "VisitorId", "CourseId" },
                principalTable: "Courses",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationCodes_Users_VisitorId_RedeemedByStudentId",
                table: "ActivationCodes",
                columns: new[] { "VisitorId", "RedeemedByStudentId" },
                principalTable: "Users",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_ClassSessions_VisitorId_SessionId",
                table: "Attendances",
                columns: new[] { "VisitorId", "SessionId" },
                principalTable: "ClassSessions",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Users_VisitorId_StudentId",
                table: "Attendances",
                columns: new[] { "VisitorId", "StudentId" },
                principalTable: "Users",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_VisitorId_ActorId",
                table: "AuditLogs",
                columns: new[] { "VisitorId", "ActorId" },
                principalTable: "Users",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSessions_StudyGroups_VisitorId_GroupId",
                table: "ClassSessions",
                columns: new[] { "VisitorId", "GroupId" },
                principalTable: "StudyGroups",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AcademicLevels_VisitorId_LevelId",
                table: "Courses",
                columns: new[] { "VisitorId", "LevelId" },
                principalTable: "AcademicLevels",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_VisitorId_TeacherId",
                table: "Courses",
                columns: new[] { "VisitorId", "TeacherId" },
                principalTable: "Users",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUnits_Courses_VisitorId_CourseId",
                table: "CourseUnits",
                columns: new[] { "VisitorId", "CourseId" },
                principalTable: "Courses",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupStudents_StudyGroups_VisitorId_GroupId",
                table: "GroupStudents",
                columns: new[] { "VisitorId", "GroupId" },
                principalTable: "StudyGroups",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupStudents_Users_VisitorId_StudentId",
                table: "GroupStudents",
                columns: new[] { "VisitorId", "StudentId" },
                principalTable: "Users",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonProgresses_Lessons_VisitorId_LessonId",
                table: "LessonProgresses",
                columns: new[] { "VisitorId", "LessonId" },
                principalTable: "Lessons",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonProgresses_Users_VisitorId_StudentId",
                table: "LessonProgresses",
                columns: new[] { "VisitorId", "StudentId" },
                principalTable: "Users",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_CourseUnits_VisitorId_UnitId",
                table: "Lessons",
                columns: new[] { "VisitorId", "UnitId" },
                principalTable: "CourseUnits",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentStudents_Users_VisitorId_ParentId",
                table: "ParentStudents",
                columns: new[] { "VisitorId", "ParentId" },
                principalTable: "Users",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentStudents_Users_VisitorId_StudentId",
                table: "ParentStudents",
                columns: new[] { "VisitorId", "StudentId" },
                principalTable: "Users",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Subscriptions_VisitorId_SubscriptionId",
                table: "Payments",
                columns: new[] { "VisitorId", "SubscriptionId" },
                principalTable: "Subscriptions",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_VisitorId_RecordedByUserId",
                table: "Payments",
                columns: new[] { "VisitorId", "RecordedByUserId" },
                principalTable: "Users",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyGroups_Courses_VisitorId_CourseId",
                table: "StudyGroups",
                columns: new[] { "VisitorId", "CourseId" },
                principalTable: "Courses",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Courses_VisitorId_CourseId",
                table: "Subscriptions",
                columns: new[] { "VisitorId", "CourseId" },
                principalTable: "Courses",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_VisitorId_StudentId",
                table: "Subscriptions",
                columns: new[] { "VisitorId", "StudentId" },
                principalTable: "Users",
                principalColumns: new[] { "VisitorId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivationCodes_Courses_VisitorId_CourseId",
                table: "ActivationCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivationCodes_Users_VisitorId_RedeemedByStudentId",
                table: "ActivationCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_ClassSessions_VisitorId_SessionId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Users_VisitorId_StudentId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_VisitorId_ActorId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassSessions_StudyGroups_VisitorId_GroupId",
                table: "ClassSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AcademicLevels_VisitorId_LevelId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_VisitorId_TeacherId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseUnits_Courses_VisitorId_CourseId",
                table: "CourseUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupStudents_StudyGroups_VisitorId_GroupId",
                table: "GroupStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupStudents_Users_VisitorId_StudentId",
                table: "GroupStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonProgresses_Lessons_VisitorId_LessonId",
                table: "LessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonProgresses_Users_VisitorId_StudentId",
                table: "LessonProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_CourseUnits_VisitorId_UnitId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentStudents_Users_VisitorId_ParentId",
                table: "ParentStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentStudents_Users_VisitorId_StudentId",
                table: "ParentStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Subscriptions_VisitorId_SubscriptionId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_VisitorId_RecordedByUserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyGroups_Courses_VisitorId_CourseId",
                table: "StudyGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Courses_VisitorId_CourseId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_VisitorId_StudentId",
                table: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "AssignmentSubmissions");

            migrationBuilder.DropTable(
                name: "ExamAnswers");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "ExamAttempts");

            migrationBuilder.DropTable(
                name: "ExamChoices");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "ExamQuestions");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_VisitorId_CourseId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_VisitorId_StudentId",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyGroups",
                table: "StudyGroups");

            migrationBuilder.DropIndex(
                name: "IX_StudyGroups_VisitorId_CourseId",
                table: "StudyGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformSettings",
                table: "PlatformSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_VisitorId_RecordedByUserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_VisitorId_SubscriptionId",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParentStudents",
                table: "ParentStudents");

            migrationBuilder.DropIndex(
                name: "IX_ParentStudents_VisitorId_StudentId",
                table: "ParentStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_VisitorId_UnitId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonProgresses",
                table: "LessonProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LessonProgresses_VisitorId_LessonId",
                table: "LessonProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LessonProgresses_VisitorId_StudentId",
                table: "LessonProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LessonProgresses_VisitorId_StudentId_LessonId",
                table: "LessonProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupStudents",
                table: "GroupStudents");

            migrationBuilder.DropIndex(
                name: "IX_GroupStudents_VisitorId_StudentId",
                table: "GroupStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DemoStates",
                table: "DemoStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseUnits",
                table: "CourseUnits");

            migrationBuilder.DropIndex(
                name: "IX_CourseUnits_VisitorId_CourseId",
                table: "CourseUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_VisitorId_LevelId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_VisitorId_TeacherId",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassSessions",
                table: "ClassSessions");

            migrationBuilder.DropIndex(
                name: "IX_ClassSessions_VisitorId_GroupId",
                table: "ClassSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_VisitorId_ActorId",
                table: "AuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_VisitorId_SessionId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_VisitorId_SessionId_StudentId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_VisitorId_StudentId",
                table: "Attendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivationCodes",
                table: "ActivationCodes");

            migrationBuilder.DropIndex(
                name: "IX_ActivationCodes_VisitorId_Code",
                table: "ActivationCodes");

            migrationBuilder.DropIndex(
                name: "IX_ActivationCodes_VisitorId_CourseId",
                table: "ActivationCodes");

            migrationBuilder.DropIndex(
                name: "IX_ActivationCodes_VisitorId_RedeemedByStudentId",
                table: "ActivationCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicLevels",
                table: "AcademicLevels");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "StudyGroups");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "PlatformSettings");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "ParentStudents");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "LessonProgresses");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "GroupStudents");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "DemoStates");

            migrationBuilder.DropColumn(
                name: "RuntimeId",
                table: "DemoStates");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "CourseUnits");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "ActivationCodes");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "AcademicLevels");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Subscriptions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "Subscriptions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Subscriptions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "StudyGroups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StudyGroups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PlatformSettings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "SubscriptionId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "RecordedByUserId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "ParentStudents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "ParentStudents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "UnitId",
                table: "Lessons",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Lessons",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "LessonProgresses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "LessonId",
                table: "LessonProgresses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "LessonProgresses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "GroupStudents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "GroupId",
                table: "GroupStudents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "DemoStates",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "CourseUnits",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CourseUnits",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "LevelId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "GroupId",
                table: "ClassSessions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ClassSessions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "EntityId",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "ActorId",
                table: "AuditLogs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AuditLogs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Attendances",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "Attendances",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Attendances",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "RedeemedByStudentId",
                table: "ActivationCodes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "ActivationCodes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ActivationCodes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AcademicLevels",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyGroups",
                table: "StudyGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformSettings",
                table: "PlatformSettings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParentStudents",
                table: "ParentStudents",
                columns: new[] { "ParentId", "StudentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonProgresses",
                table: "LessonProgresses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupStudents",
                table: "GroupStudents",
                columns: new[] { "GroupId", "StudentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DemoStates",
                table: "DemoStates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseUnits",
                table: "CourseUnits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassSessions",
                table: "ClassSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivationCodes",
                table: "ActivationCodes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicLevels",
                table: "AcademicLevels",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CourseId",
                table: "Subscriptions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_StudentId",
                table: "Subscriptions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyGroups_CourseId",
                table: "StudyGroups",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RecordedByUserId",
                table: "Payments",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentStudents_StudentId",
                table: "ParentStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_UnitId",
                table: "Lessons",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgresses_LessonId",
                table: "LessonProgresses",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgresses_StudentId_LessonId",
                table: "LessonProgresses",
                columns: new[] { "StudentId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupStudents_StudentId",
                table: "GroupStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseUnits_CourseId",
                table: "CourseUnits",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_LevelId",
                table: "Courses",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_GroupId",
                table: "ClassSessions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActorId",
                table: "AuditLogs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SessionId_StudentId",
                table: "Attendances",
                columns: new[] { "SessionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCodes_Code",
                table: "ActivationCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCodes_CourseId",
                table: "ActivationCodes",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCodes_RedeemedByStudentId",
                table: "ActivationCodes",
                column: "RedeemedByStudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationCodes_Courses_CourseId",
                table: "ActivationCodes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationCodes_Users_RedeemedByStudentId",
                table: "ActivationCodes",
                column: "RedeemedByStudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_ClassSessions_SessionId",
                table: "Attendances",
                column: "SessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Users_StudentId",
                table: "Attendances",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_ActorId",
                table: "AuditLogs",
                column: "ActorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSessions_StudyGroups_GroupId",
                table: "ClassSessions",
                column: "GroupId",
                principalTable: "StudyGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AcademicLevels_LevelId",
                table: "Courses",
                column: "LevelId",
                principalTable: "AcademicLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_TeacherId",
                table: "Courses",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUnits_Courses_CourseId",
                table: "CourseUnits",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupStudents_StudyGroups_GroupId",
                table: "GroupStudents",
                column: "GroupId",
                principalTable: "StudyGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupStudents_Users_StudentId",
                table: "GroupStudents",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonProgresses_Lessons_LessonId",
                table: "LessonProgresses",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonProgresses_Users_StudentId",
                table: "LessonProgresses",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_CourseUnits_UnitId",
                table: "Lessons",
                column: "UnitId",
                principalTable: "CourseUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentStudents_Users_ParentId",
                table: "ParentStudents",
                column: "ParentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentStudents_Users_StudentId",
                table: "ParentStudents",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Subscriptions_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_RecordedByUserId",
                table: "Payments",
                column: "RecordedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyGroups_Courses_CourseId",
                table: "StudyGroups",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Courses_CourseId",
                table: "Subscriptions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_StudentId",
                table: "Subscriptions",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
