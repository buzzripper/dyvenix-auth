using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dyvenix.Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AuthMode = table.Column<int>(type: "int", nullable: false),
                    ExternalAuthority = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalClientId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExternalClientSecret = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ADDcHost = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ADDomain = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ADLdapPort = table.Column<int>(type: "int", nullable: true),
                    ADBaseDn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            //// Seed the default tenant so existing users can be assigned to it before the FK is enforced.
            //migrationBuilder.Sql(@"
            //    INSERT INTO [Tenant] ([Id], [Name], [Slug], [AuthMethod], [IsActive], [CreatedAt])
            //    VALUES ('A1000000-0000-0000-0000-000000000001', 'Acme Corp', 'acme', 'Local', 1, SYSUTCDATETIME());
            //");

            //migrationBuilder.Sql(@"
            //    UPDATE [AspNetUsers] SET [TenantId] = 'A1000000-0000-0000-0000-000000000001'
            //    WHERE [TenantId] = '00000000-0000-0000-0000-000000000000';
            //");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId_Email",
                table: "AspNetUsers",
                columns: new[] { "TenantId", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_Slug",
                table: "Tenant",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tenant_TenantId",
                table: "AspNetUsers",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Tenant_TenantId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TenantId_Email",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AspNetUsers");
        }
    }
}
