
namespace FileUploader.Core.Models
{
    public class ChunkMetadata
    {
        public string Filename { get; set; }
        public int ChunkNumber { get; set; }

        public ChunkMetadata(string filename, int chunkNumber)
        {
            Filename = filename;
            ChunkNumber = chunkNumber;
        }
    }
}
