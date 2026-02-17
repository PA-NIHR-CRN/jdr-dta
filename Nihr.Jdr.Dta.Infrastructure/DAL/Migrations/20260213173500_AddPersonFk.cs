using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nihr.Jdr.Dta.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Symptom",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Moca",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Mmse",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Diagnosis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Ace",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Symptom_PersonId",
                table: "Symptom",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moca_PersonId",
                table: "Moca",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mmse_PersonId",
                table: "Mmse",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diagnosis_PersonId",
                table: "Diagnosis",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PersonId",
                table: "Appointment",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ace_PersonId",
                table: "Ace",
                column: "PersonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ace_Person_PersonId",
                table: "Ace",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Person_PersonId",
                table: "Appointment",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnosis_Person_PersonId",
                table: "Diagnosis",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mmse_Person_PersonId",
                table: "Mmse",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Moca_Person_PersonId",
                table: "Moca",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Symptom_Person_PersonId",
                table: "Symptom",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ace_Person_PersonId",
                table: "Ace");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Person_PersonId",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_Diagnosis_Person_PersonId",
                table: "Diagnosis");

            migrationBuilder.DropForeignKey(
                name: "FK_Mmse_Person_PersonId",
                table: "Mmse");

            migrationBuilder.DropForeignKey(
                name: "FK_Moca_Person_PersonId",
                table: "Moca");

            migrationBuilder.DropForeignKey(
                name: "FK_Symptom_Person_PersonId",
                table: "Symptom");

            migrationBuilder.DropIndex(
                name: "IX_Symptom_PersonId",
                table: "Symptom");

            migrationBuilder.DropIndex(
                name: "IX_Moca_PersonId",
                table: "Moca");

            migrationBuilder.DropIndex(
                name: "IX_Mmse_PersonId",
                table: "Mmse");

            migrationBuilder.DropIndex(
                name: "IX_Diagnosis_PersonId",
                table: "Diagnosis");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_PersonId",
                table: "Appointment");

            migrationBuilder.DropIndex(
                name: "IX_Ace_PersonId",
                table: "Ace");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Symptom");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Moca");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Mmse");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Diagnosis");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Ace");
        }
    }
}
