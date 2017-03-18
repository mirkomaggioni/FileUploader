namespace FileUploader.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBufferFieldFromFileBlobTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.FileBlobs", "File");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FileBlobs", "File", c => c.Binary());
        }
    }
}
