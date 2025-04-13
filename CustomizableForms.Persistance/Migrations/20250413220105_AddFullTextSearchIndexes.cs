using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomizableForms.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE INDEX idx_templates_fulltext ON ""Templates"" 
                USING gin(to_tsvector('russian', ""Title"" || ' ' || COALESCE(""Description"", '')));
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX idx_questions_fulltext ON ""Questions"" 
                USING gin(to_tsvector('russian', ""Title"" || ' ' || COALESCE(""Description"", '')));
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX idx_templatecomments_fulltext ON ""TemplateComments"" 
                USING gin(to_tsvector('russian', ""Content""));
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX idx_tags_fulltext ON ""Tags"" 
                USING gin(to_tsvector('russian', ""Name""));
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_templates_fulltext;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_questions_fulltext;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_templatecomments_fulltext;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_tags_fulltext;");
        }
    }
}
