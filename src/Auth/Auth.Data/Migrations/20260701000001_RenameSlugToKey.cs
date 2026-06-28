using Dyvenix.Auth.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dyvenix.Auth.Data.Migrations
{
	[DbContext(typeof(AuthDbContext))]
	[Migration("20260701000001_RenameSlugToKey")]
	public partial class RenameSlugToKey : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Add the new Key column (nullable initially so existing rows can be populated)
			migrationBuilder.AddColumn<string>(
				name: "Key",
				table: "Tenant",
				type: "nvarchar(100)",
				maxLength: 100,
				nullable: true);

			// Copy existing Slug values into Key
			migrationBuilder.Sql("UPDATE [Tenant] SET [Key] = [Slug]");

			// Make Key NOT NULL now that all rows have a value
			migrationBuilder.AlterColumn<string>(
				name: "Key",
				table: "Tenant",
				type: "nvarchar(100)",
				maxLength: 100,
				nullable: false,
				defaultValue: "");

			// Drop the old Slug column (drop its index first if it exists)
			migrationBuilder.Sql("IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Tenant_Slug' AND object_id = OBJECT_ID('Tenant')) DROP INDEX [IX_Tenant_Slug] ON [Tenant]");
			migrationBuilder.DropColumn(
				name: "Slug",
				table: "Tenant");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Re-add Slug column
			migrationBuilder.AddColumn<string>(
				name: "Slug",
				table: "Tenant",
				type: "nvarchar(100)",
				maxLength: 100,
				nullable: true);

			// Copy Key values back to Slug
			migrationBuilder.Sql("UPDATE [Tenant] SET [Slug] = [Key]");

			// Make Slug NOT NULL
			migrationBuilder.AlterColumn<string>(
				name: "Slug",
				table: "Tenant",
				type: "nvarchar(100)",
				maxLength: 100,
				nullable: false,
				defaultValue: "");

			// Drop the Key column
			migrationBuilder.DropColumn(
				name: "Key",
				table: "Tenant");
		}
	}
}
