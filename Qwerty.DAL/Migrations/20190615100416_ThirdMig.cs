using Microsoft.EntityFrameworkCore.Migrations;

namespace Qwerty.DAL.Migrations
{
    public partial class ThirdMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(name: "FK_AspNetRoleClaims_AspNetRoles_RoleId", table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddColumn<string>(name: "ProviderDisplayName", table: "AspNetUserLogins", nullable: true);
         

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId", "AspNetRoleClaims");
            migrationBuilder.DropColumn("ProviderDisplayName", "AspNetUserLogins");
        }
    }
}
