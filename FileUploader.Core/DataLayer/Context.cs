using System.Data.Entity;

namespace FileUploader.Core.DataLayer
{
    public class Context : DbContext
    {
        public Context() : base() { }
        public DbSet<FileBlob> FileBlobs { get; set; }
    }
}
