using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCommerceConsumerPlayground.Migrations
{
    /// <inheritdoc />
    public partial class InitDb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Orders_OrderId",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Items",
                newName: "totalPrice");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Items",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "OfferingId",
                table: "Items",
                newName: "offeringId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Items",
                newName: "itemId");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "itemState",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "shoppingBasketId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Orders_OrderId",
                table: "Items",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Orders_OrderId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "itemState",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "shoppingBasketId",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "totalPrice",
                table: "Items",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "Items",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "offeringId",
                table: "Items",
                newName: "OfferingId");

            migrationBuilder.RenameColumn(
                name: "itemId",
                table: "Items",
                newName: "ItemId");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Orders_OrderId",
                table: "Items",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
