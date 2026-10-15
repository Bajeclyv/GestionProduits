using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestionProduits.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "t_e_marque_mqe",
                schema: "public",
                columns: table => new
                {
                    mqe_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mqe_nommarque = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_marque_mqe", x => x.mqe_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_typeproduit_tpt",
                schema: "public",
                columns: table => new
                {
                    tpt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tpt_nomtypeproduit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_typeproduit_tpt", x => x.tpt_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_produit_pdt",
                schema: "public",
                columns: table => new
                {
                    pdt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pdt_nomproduit = table.Column<string>(type: "text", nullable: false),
                    pdt_description = table.Column<string>(type: "text", nullable: true),
                    pdt_nomphoto = table.Column<string>(type: "text", nullable: true),
                    pdt_uriphoto = table.Column<string>(type: "text", nullable: true),
                    TypeProduitId = table.Column<int>(type: "integer", nullable: true),
                    MarqueId = table.Column<int>(type: "integer", nullable: true),
                    pdt_stockreel = table.Column<int>(type: "integer", nullable: false),
                    pdt_stockmin = table.Column<int>(type: "integer", nullable: false),
                    pdt_stockmax = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_produit_pdt", x => x.pdt_id);
                    table.ForeignKey(
                        name: "FK_t_e_produit_pdt_t_e_marque_mqe_TypeProduitId",
                        column: x => x.TypeProduitId,
                        principalSchema: "public",
                        principalTable: "t_e_marque_mqe",
                        principalColumn: "mqe_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_e_produit_pdt_t_e_typeproduit_tpt_MarqueId",
                        column: x => x.MarqueId,
                        principalSchema: "public",
                        principalTable: "t_e_typeproduit_tpt",
                        principalColumn: "tpt_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_produit_pdt_MarqueId",
                schema: "public",
                table: "t_e_produit_pdt",
                column: "MarqueId");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_produit_pdt_pdt_nomproduit",
                schema: "public",
                table: "t_e_produit_pdt",
                column: "pdt_nomproduit");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_produit_pdt_TypeProduitId",
                schema: "public",
                table: "t_e_produit_pdt",
                column: "TypeProduitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_e_produit_pdt",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_marque_mqe",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_typeproduit_tpt",
                schema: "public");
        }
    }
}
