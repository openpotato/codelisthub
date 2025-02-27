using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeListHub.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Unique Id"),
                    Identifier_Value = table.Column<string>(type: "text", nullable: true, comment: "The identifier value."),
                    Identifier_Source_LongName = table.Column<string>(type: "text", nullable: true, comment: "Human-readable name for the source."),
                    Identifier_Source_ShortName = table.Column<string>(type: "text", nullable: true, comment: "Short name of the source."),
                    Identifier_Source_Url = table.Column<string>(type: "text", nullable: true, comment: "More information about the source."),
                    LongName = table.Column<string>(type: "text", nullable: true, comment: "Human-readable name for the publisher"),
                    ShortName = table.Column<string>(type: "text", nullable: true, comment: "Short name for the publisher"),
                    Url = table.Column<string>(type: "text", nullable: true, comment: "Url with further information")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                },
                comment: "Publishers responsible for publishing and/or maintaining the codes");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Unique Id"),
                    Value = table.Column<string>(type: "text", nullable: true, comment: "The tag value.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                },
                comment: "Tags of documents");

            migrationBuilder.CreateTable(
                name: "DocumentInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Unique Id"),
                    CanonicalUri = table.Column<string>(type: "text", nullable: true, comment: "Canonical URI which uniquely identifies all versions this document (collectively)"),
                    CanonicalVersionUri = table.Column<string>(type: "text", nullable: false, comment: "Canonical URI which uniquely identifies this document"),
                    DocumentType = table.Column<int>(type: "integer", nullable: false, comment: "The document type"),
                    Language = table.Column<string>(type: "text", nullable: false, comment: "A short identifier of this document"),
                    LongName = table.Column<string>(type: "text", nullable: true, comment: "Human-readable name of this document"),
                    PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "The timepoint of the publication of the document."),
                    ShortName = table.Column<string>(type: "text", nullable: false, comment: "A short identifier of this document"),
                    ValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "The timepoint from which this document is valid."),
                    ValidTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "The timepoint until which this document is valid."),
                    Version = table.Column<string>(type: "text", nullable: true, comment: "The version of the document"),
                    PublisherId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Reference to publisher")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentInfo_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Index entries for code list documents and code list set documents");

            migrationBuilder.CreateTable(
                name: "DocumentFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Unique Id"),
                    FilePath = table.Column<string>(type: "text", nullable: false, comment: "The file path"),
                    MetaOnly = table.Column<bool>(type: "boolean", nullable: false, comment: "Just metadata without data?"),
                    MediaType = table.Column<string>(type: "text", nullable: true, comment: "Media type of the file"),
                    DocumentInfoId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Reference to DocumentInfo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_DocumentInfo_DocumentInfoId",
                        column: x => x.DocumentInfoId,
                        principalTable: "DocumentInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "File representations of code list documents or code list set documents");

            migrationBuilder.CreateTable(
                name: "DocumentInfoTags",
                columns: table => new
                {
                    DocumentInfosId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentInfoTags", x => new { x.DocumentInfosId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_DocumentInfoTags_DocumentInfo_DocumentInfosId",
                        column: x => x.DocumentInfosId,
                        principalTable: "DocumentInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentInfoTags_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Join table between DocumentInfo and Tags");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_DocumentInfoId",
                table: "DocumentFiles",
                column: "DocumentInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfo_Language_CanonicalVersionUri",
                table: "DocumentInfo",
                columns: new[] { "Language", "CanonicalVersionUri" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfo_PublisherId",
                table: "DocumentInfo",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfo_ShortName",
                table: "DocumentInfo",
                column: "ShortName");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInfoTags_TagsId",
                table: "DocumentInfoTags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_ShortName",
                table: "Publishers",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Value",
                table: "Tags",
                column: "Value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentFiles");

            migrationBuilder.DropTable(
                name: "DocumentInfoTags");

            migrationBuilder.DropTable(
                name: "DocumentInfo");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Publishers");
        }
    }
}
