using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dyvenix.Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantApplicationFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_TenantApplication_TenantId_Tenant",
                table: "TenantApplication",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantApplication_TenantId_Tenant",
                table: "TenantApplication");
        }
    }
}
