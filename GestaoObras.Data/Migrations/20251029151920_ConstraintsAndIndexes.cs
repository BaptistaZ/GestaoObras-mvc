using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoObras.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConstraintsAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimentosMaterial_Materiais_MaterialId",
                table: "MovimentosMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_Obras_Clientes_ClienteId",
                table: "Obras");

            migrationBuilder.DropIndex(
                name: "IX_Obras_ClienteId",
                table: "Obras");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Pagamentos",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Pagamentos",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Obras",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Morada",
                table: "Obras",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Obras",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Materiais",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Materiais",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "MaosDeObra",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "HorasTrabalhadas",
                table: "MaosDeObra",
                type: "numeric(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Clientes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Clientes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "NIF",
                table: "Clientes",
                type: "character varying(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Morada",
                table: "Clientes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Clientes",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_DataHora",
                table: "Pagamentos",
                column: "DataHora");

            migrationBuilder.CreateIndex(
                name: "IX_Obras_ClienteId_Nome",
                table: "Obras",
                columns: new[] { "ClienteId", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimentosMaterial_DataHora",
                table: "MovimentosMaterial",
                column: "DataHora");

            migrationBuilder.CreateIndex(
                name: "IX_Materiais_Nome",
                table: "Materiais",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaosDeObra_DataHora",
                table: "MaosDeObra",
                column: "DataHora");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_NIF",
                table: "Clientes",
                column: "NIF",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentosMaterial_Materiais_MaterialId",
                table: "MovimentosMaterial",
                column: "MaterialId",
                principalTable: "Materiais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Obras_Clientes_ClienteId",
                table: "Obras",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimentosMaterial_Materiais_MaterialId",
                table: "MovimentosMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_Obras_Clientes_ClienteId",
                table: "Obras");

            migrationBuilder.DropIndex(
                name: "IX_Pagamentos_DataHora",
                table: "Pagamentos");

            migrationBuilder.DropIndex(
                name: "IX_Obras_ClienteId_Nome",
                table: "Obras");

            migrationBuilder.DropIndex(
                name: "IX_MovimentosMaterial_DataHora",
                table: "MovimentosMaterial");

            migrationBuilder.DropIndex(
                name: "IX_Materiais_Nome",
                table: "Materiais");

            migrationBuilder.DropIndex(
                name: "IX_MaosDeObra_DataHora",
                table: "MaosDeObra");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Email",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_NIF",
                table: "Clientes");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Pagamentos",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Pagamentos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Obras",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Morada",
                table: "Obras",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Obras",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Materiais",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Materiais",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "MaosDeObra",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<decimal>(
                name: "HorasTrabalhadas",
                table: "MaosDeObra",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(9,2)",
                oldPrecision: 9,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "NIF",
                table: "Clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(9)",
                oldMaxLength: 9);

            migrationBuilder.AlterColumn<string>(
                name: "Morada",
                table: "Clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.CreateIndex(
                name: "IX_Obras_ClienteId",
                table: "Obras",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimentosMaterial_Materiais_MaterialId",
                table: "MovimentosMaterial",
                column: "MaterialId",
                principalTable: "Materiais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Obras_Clientes_ClienteId",
                table: "Obras",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
