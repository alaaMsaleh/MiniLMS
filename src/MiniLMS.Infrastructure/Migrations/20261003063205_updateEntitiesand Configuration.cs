using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniLMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateEntitiesandConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "QuizQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionId1",
                table: "QuizQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Questions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Questions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuizSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    IncorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizSubmissions_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizSubmissions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizSubmissionId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedChoiceId = table.Column<int>(type: "int", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    QuestionTextSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SelectedChoiceTextSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectChoiceId = table.Column<int>(type: "int", nullable: false),
                    CorrectChoiceTextSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswer_Choices_SelectedChoiceId",
                        column: x => x.SelectedChoiceId,
                        principalTable: "Choices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentAnswer_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAnswer_QuizSubmissions_QuizSubmissionId",
                        column: x => x.QuizSubmissionId,
                        principalTable: "QuizSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_QuestionId1",
                table: "QuizQuestions",
                column: "QuestionId1");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmissions_QuizId",
                table: "QuizSubmissions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmissions_StudentId",
                table: "QuizSubmissions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswer_QuestionId",
                table: "StudentAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswer_QuizSubmissionId",
                table: "StudentAnswer",
                column: "QuizSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswer_SelectedChoiceId",
                table: "StudentAnswer",
                column: "SelectedChoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_Questions_QuestionId1",
                table: "QuizQuestions",
                column: "QuestionId1",
                principalTable: "Questions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_Questions_QuestionId1",
                table: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "StudentAnswer");

            migrationBuilder.DropTable(
                name: "QuizSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestions_QuestionId1",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "QuestionId1",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Questions");
        }
    }
}
