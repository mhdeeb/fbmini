using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fbmini.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserData_Files_CoverId",
                table: "UserData");

            migrationBuilder.DropForeignKey(
                name: "FK_UserData_Files_PictureId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_UserData_CoverId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_UserData_PictureId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserDataId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<byte[]>(
                name: "FileData",
                table: "Files",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "Files",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PosterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentPostId = table.Column<int>(type: "int", nullable: true),
                    AttachmentId = table.Column<int>(type: "int", nullable: true),
                    UserDataId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_PosterId",
                        column: x => x.PosterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posts_Files_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_Posts_ParentPostId",
                        column: x => x.ParentPostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_UserData_UserDataId",
                        column: x => x.UserDataId,
                        principalTable: "UserData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PostDislikers",
                columns: table => new
                {
                    DislikedPostsId = table.Column<int>(type: "int", nullable: false),
                    DislikersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostDislikers", x => new { x.DislikedPostsId, x.DislikersId });
                    table.ForeignKey(
                        name: "FK_PostDislikers_AspNetUsers_DislikersId",
                        column: x => x.DislikersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostDislikers_Posts_DislikedPostsId",
                        column: x => x.DislikedPostsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostLikers",
                columns: table => new
                {
                    LikedPostsId = table.Column<int>(type: "int", nullable: false),
                    LikersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostLikers", x => new { x.LikedPostsId, x.LikersId });
                    table.ForeignKey(
                        name: "FK_PostLikers_AspNetUsers_LikersId",
                        column: x => x.LikersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostLikers_Posts_LikedPostsId",
                        column: x => x.LikedPostsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserData_CoverId",
                table: "UserData",
                column: "CoverId");

            migrationBuilder.CreateIndex(
                name: "IX_UserData_PictureId",
                table: "UserData",
                column: "PictureId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserDataId",
                table: "AspNetUsers",
                column: "UserDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostDislikers_DislikersId",
                table: "PostDislikers",
                column: "DislikersId");

            migrationBuilder.CreateIndex(
                name: "IX_PostLikers_LikersId",
                table: "PostLikers",
                column: "LikersId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AttachmentId",
                table: "Posts",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ParentPostId",
                table: "Posts",
                column: "ParentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_PosterId",
                table: "Posts",
                column: "PosterId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserDataId",
                table: "Posts",
                column: "UserDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_Files_CoverId",
                table: "UserData",
                column: "CoverId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_Files_PictureId",
                table: "UserData",
                column: "PictureId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserData_Files_CoverId",
                table: "UserData");

            migrationBuilder.DropForeignKey(
                name: "FK_UserData_Files_PictureId",
                table: "UserData");

            migrationBuilder.DropTable(
                name: "PostDislikers");

            migrationBuilder.DropTable(
                name: "PostLikers");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_UserData_CoverId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_UserData_PictureId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserDataId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserData");

            migrationBuilder.AlterColumn<byte[]>(
                name: "FileData",
                table: "Files",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "Files",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_UserData_CoverId",
                table: "UserData",
                column: "CoverId",
                unique: true,
                filter: "[CoverId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserData_PictureId",
                table: "UserData",
                column: "PictureId",
                unique: true,
                filter: "[PictureId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserDataId",
                table: "AspNetUsers",
                column: "UserDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_Files_CoverId",
                table: "UserData",
                column: "CoverId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_Files_PictureId",
                table: "UserData",
                column: "PictureId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
