using Microsoft.EntityFrameworkCore.Migrations;

namespace ClientImport.Migrations
{
    public partial class Initial : Migration
    {
        #region SQL
        private const string C_CreateLogTableSQL = @"CREATE TABLE [dbo].[Log](
	[Time] [datetime] NOT NULL,
	[Event] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO";

        private const string C_DeleteLogTableSQL = @"DROP TABLE [dbo].[Log]
GO";

        private const string C_CreateTrigersSQL = @"CREATE TRIGGER TRG_ClientUpdated
ON dbo.Clients AFTER UPDATE 
AS BEGIN
      INSERT INTO 
          dbo.Log(Time, Event)
          SELECT
             GETDATE(), 'Pakestas esamas klientas. Iš: [' + CAST(d.Id AS VARCHAR(11)) + ', ' + d.Name + ', ' + d.Address + ', ' + ISNULL(d.PostCode, 'null') + '] į: [' + CAST(i.Id AS VARCHAR(11)) + ', ' + i.Name + ', ' + i.Address + ', ' + ISNULL(i.PostCode, 'null') + ']'
          FROM 
             DELETED d
			 full join 
			 INSERTED i on d.Id = i.Id
END
GO


CREATE TRIGGER TRG_ClientInserted
ON dbo.Clients AFTER INSERT 
AS BEGIN
      INSERT INTO 
          dbo.Log(Time, Event)
          SELECT
             GETDATE(), 'Pridėtas naujas klientas: [' + CAST(i.Id AS VARCHAR(11)) + ', ' + i.Name + ', ' + i.Address + ', ' + ISNULL(i.PostCode, 'null') + ']'
          FROM 
             INSERTED i
END
GO";

        private const string C_DeleteTrigersSQL = @"DROP TRIGGER [dbo].[TRG_ClientInserted]
GO

DROP TRIGGER [dbo].[TRG_ClientUpdated]
GO"; 
        #endregion

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PostCode = table.Column<string>(type: "nvarchar(31)", maxLength: 31, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientName",
                table: "Clients",
                column: "Name",
                unique: true);
            migrationBuilder.Sql(C_CreateLogTableSQL);
            migrationBuilder.Sql(C_CreateTrigersSQL);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(C_DeleteTrigersSQL);
            migrationBuilder.Sql(C_DeleteLogTableSQL);
            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
