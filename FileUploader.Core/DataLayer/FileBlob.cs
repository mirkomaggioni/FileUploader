using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileUploader.Core.DataLayer
{
    [Table("FileBlobs")]
    public class FileBlob
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
    }
}
