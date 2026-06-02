using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dyvenix.Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdToRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AspNetRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_TenantId_NormalizedName",
                table: "AspNetRoles",
                columns: new[] { "TenantId", "NormalizedName" },
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_Tenant_TenantId",
                table: "AspNetRoles",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_Tenant_TenantId",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_TenantId_NormalizedName",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AspNetUserRoles",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_TenantId_UserId_RoleId",
                table: "AspNetUserRoles",
                columns: new[] { "TenantId", "UserId", "RoleId" });
        }
    }
}
