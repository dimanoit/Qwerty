using Microsoft.EntityFrameworkCore.Migrations;

namespace Qwerty.DAL.Migrations
{
    public partial class AddKeysToAspNetRoleClaimsAndCreateAspNetUserTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(name: "PK_AspNetRoleClaims", column: "Id", table: "AspNetRoleClaims");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_AspNetRoleClaims", "AspNetRoleClaims");
        }
    }
}
