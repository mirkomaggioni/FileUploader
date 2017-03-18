namespace FileUploader.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileBlobs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        File = c.Binary(),
                        Size = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FileBlobs");
        }
    }
}
