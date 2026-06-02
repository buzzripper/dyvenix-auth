using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dyvenix.Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantApplication",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantApplication", x => new { x.TenantId, x.ClientId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantApplication_ClientId",
                table: "TenantApplication",
                column: "ClientId");

            migrationBuilder.DropTable(
                name: "FidoStoredCredential");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantApplication");

            migrationBuilder.CreateTable(
                name: "FidoStoredCredential",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AaGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CredType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptorJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicKey = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignatureCounter = table.Column<long>(type: "bigint", nullable: false),
                    UserHandle = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserId = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FidoStoredCredential", x => x.Id);
                });
        }
    }
}
