using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploader.Core.Models
{
    public interface IUploadSession
    {
        List<ChunkMetadata> Chunks { get; set; }
        string Filename { get; }
        long Filesize { get; }
        bool AddChunk(string filename, string chunkFileName, int chunkNumber, int totalChunks);
        Task MergeChunks(string path);
    }

    public class UploadSession : IUploadSession
    {
        public string Filename { get; private set; }
        public long Filesize { get; private set; }
        private int _totalChunks;
        private int _chunksUploaded;

        public List<ChunkMetadata> Chunks { get; set; }

        public UploadSession()
        {
            Filesize = 0;
            _chunksUploaded = 0;
            Chunks = new List<ChunkMetadata>();
        }

        public bool AddChunk(string filename, string chunkFileName, int chunkNumber, int totalChunks)
        {
            if (Filename == null)
            {
                Filename = filename;
                _totalChunks = totalChunks;
            }

            var metadata = new ChunkMetadata(chunkFileName, chunkNumber);
            Chunks.Add(metadata);

            _chunksUploaded++;
            return _chunksUploaded == _totalChunks;
        }

        public async Task MergeChunks(string path)
        {
            var filePath = path + Filename;

            using (var mainFile = new FileStream(filePath, FileMode.Create))
            {
                foreach (var chunk in Chunks.OrderBy(c => c.ChunkNumber))
                {
                    using (var chunkFile = new FileStream(chunk.Filename, FileMode.Open))
                    {
                        await chunkFile.CopyToAsync(mainFile);
                        Filesize += chunkFile.Length;
                    }
                }
            }

            foreach (var chunk in Chunks)
            {
                File.Delete(chunk.Filename);
            }
        }
    }
}
