using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FileUploader.Core.DataLayer;
using FileUploader.Core.Models;
using FileUploader.Core.Providers;
using Context = FileUploader.Core.DataLayer.Context;

namespace FileUploader.Core.ServiceLayer
{
    public interface IFileBlobsService
    {
        Task<FileBlob> GetFileBlob(Guid idFileBlob);
        List<FileBlob> GetFileBlobs();
        Guid StartNewSession();
        Task<bool> UploadChunk(HttpRequestMessage request);
    }

    public class FileBlobsService : IFileBlobsService
    {
        private readonly Context _db = new Context();
        private readonly string _path;
        private readonly Dictionary<string, UploadSession> _uploadSessions;

        public FileBlobsService(string path)
        {
            _path = path;
            _uploadSessions = new Dictionary<string, UploadSession>();
        }

        public List<FileBlob> GetFileBlobs()
        {
            var fileBlobs = _db.FileBlobs.ToList();

            return fileBlobs.Select(e => new FileBlob()
            {
                Id = e.Id,
                Name = e.Name,
                Size = e.Size
            }).ToList();
        }

        public async Task<FileBlob> GetFileBlob(Guid idFileBlob)
        {
            return await _db.FileBlobs.FirstOrDefaultAsync(e => e.Id == idFileBlob);
        }

        public async Task<bool> UploadChunk(HttpRequestMessage request)
        {
            var provider = new CustomMultipartFormDataStreamProvider(_path);
            await request.Content.ReadAsMultipartAsync(provider);
            provider.ExtractValues();

            var uploadSession = _uploadSessions[provider.CorrelationId];
            var completed = uploadSession.AddChunk(provider.Filename, provider.ChunkFilename, provider.ChunkNumber, provider.TotalChunks);

            if (completed)
            {
                await uploadSession.MergeChunks(_path);

                var fileBlob = new FileBlob()
                {
                    Id = Guid.NewGuid(),
                    Path = _path + uploadSession.Filename,
                    Name = uploadSession.Filename,
                    Size = uploadSession.Filesize
                };

                _db.FileBlobs.Add(fileBlob);
                await _db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public Guid StartNewSession()
        {
            var correlationId = Guid.NewGuid();
            var session = new UploadSession();
            _uploadSessions.Add(correlationId.ToString(), session);

            return correlationId;
        }
    }
}
