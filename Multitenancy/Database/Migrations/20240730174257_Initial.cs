using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Multitenancy.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "multitenancy");

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "multitenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "authors",
                schema: "multitenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_authors", x => x.id);
                    table.ForeignKey(
                        name: "fk_authors_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "multitenancy",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "multitenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "multitenancy",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "books",
                schema: "multitenancy",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_books", x => x.id);
                    table.ForeignKey(
                        name: "fk_books_authors_author_id",
                        column: x => x.author_id,
                        principalSchema: "multitenancy",
                        principalTable: "authors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_books_tenant_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "multitenancy",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_authors_name",
                schema: "multitenancy",
                table: "authors",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_authors_tenant_id",
                schema: "multitenancy",
                table: "authors",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_books_author_id",
                schema: "multitenancy",
                table: "books",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_books_tenant_id",
                schema: "multitenancy",
                table: "books",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_books_title",
                schema: "multitenancy",
                table: "books",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_name",
                schema: "multitenancy",
                table: "tenants",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                schema: "multitenancy",
                table: "users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_users_tenant_id",
                schema: "multitenancy",
                table: "users",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "books",
                schema: "multitenancy");

            migrationBuilder.DropTable(
                name: "users",
                schema: "multitenancy");

            migrationBuilder.DropTable(
                name: "authors",
                schema: "multitenancy");

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "multitenancy");
        }
    }
}
