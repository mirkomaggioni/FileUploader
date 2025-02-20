﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FileUploader.Core.DataLayer;
using FileUploader.Core.Models;
using FileUploader.Core.Providers;
using Context = FileUploader.Core.DataLayer.Context;

namespace FileUploader.Core.ServiceLayer
{
    public interface IUploadService
    {
        Task<FileBlob> GetFileBlob(Guid idFileBlob);
        Task<List<FileBlob>> GetFileBlobs();
        Guid StartNewSession();
        Task<bool> UploadChunk(HttpRequestMessage request);
    }

    public class UploadService : IUploadService
    {
        private readonly Context _db = new Context();
        private readonly string _path;
        private readonly ConcurrentDictionary<string, UploadSession> _uploadSessions;

        public UploadService(string path)
        {
            _path = path;
            _uploadSessions = new ConcurrentDictionary<string, UploadSession>();
        }

        public async Task<List<FileBlob>> GetFileBlobs()
        {
            var fileBlobs = await  _db.FileBlobs.ToListAsync();

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

            UploadSession uploadSession;
            _uploadSessions.TryGetValue(provider.CorrelationId, out uploadSession);

            if (uploadSession == null)
                throw new ObjectNotFoundException();

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
            _uploadSessions.TryAdd(correlationId.ToString(), session);

            return correlationId;
        }
    }
}
