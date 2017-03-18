namespace FileUploader.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeSizeTypeInFileBlobTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FileBlobs", "Size", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FileBlobs", "Size", c => c.Int(nullable: false));
        }
    }
}
