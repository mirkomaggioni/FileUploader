namespace FileUploader.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPathInFileBlobTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileBlobs", "Path", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileBlobs", "Path");
        }
    }
}
