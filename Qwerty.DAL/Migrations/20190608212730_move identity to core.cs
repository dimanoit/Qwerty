using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Qwerty.DAL.Migrations
{
    public partial class moveidentitytocore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "NormalizedName", table: "AspNetRoles", nullable: false, maxLength: 256, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "ConcurrencyStamp", table: "AspNetRoles", nullable: true);

            migrationBuilder.AddColumn<string>(name: "NormalizedName", table: "AspNetUsers", nullable: false, maxLength: 256, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "NormalizedEmail", table: "AspNetUsers", nullable: true, maxLength: 256);
            migrationBuilder.AddColumn<string>(name: "ConcurrencyStamp", table: "AspNetUsers", nullable: true);
            migrationBuilder.RenameColumn(name: "LockoutEndDateUtc", table: "AspNetUsers", newName: "LockoutEnd");
            migrationBuilder.AlterColumn<DateTimeOffset>(name: "LockoutEnd", table: "AspNetUsers", nullable: true);

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                });
        }

    }
}
