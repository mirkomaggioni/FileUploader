using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FileUploader.Core.DataLayer;
using FileUploader.Core.ServiceLayer;
using FluentAssertions;
using NUnit.Framework;

namespace FileUploader.Core.Tests
{
    [TestFixture]
    public class UploadServiceTests
    {
        private readonly Context _db = new Context();
        private IUploadService _uploadService;
        private string _outputPath = @"c:\temp\chunks\";
        private readonly Dictionary<string, string[]> _chunks = new Dictionary<string, string[]>()
        {
            {
                "Programming C#.pdf",
                new []
                {
                    "BodyPart_11ff84db-7892-4d04-b1f3-141f2493207f",
                    "BodyPart_41417c43-9554-4d1f-b5f8-fdb371ce3d26",
                    "BodyPart_d8d9de06-5ffc-4829-aad3-54ec1be85d49",
                    "BodyPart_e0b1d367-2df5-4bbb-8ade-9133b372f980"
                }
            },
            {
                "70-532.pdf",
                new []
                {
                    "BodyPart_2c2d686c-a41f-407c-b3de-8b1e28e18aaf",
                    "BodyPart_3e8c0ad1-6fe6-4dd6-b494-36024f2856ff",
                    "BodyPart_04a57bd5-38bf-4520-870e-4327bc86dc50",
                    "BodyPart_86bb7aca-3fef-4416-a7a8-6eb9e88db019"
                }
            }
        };

        private readonly string _filepath = $@"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}\files\";
        private List<FileBlob> _fileBlobs;

        [SetUp]
        public async Task Setup()
        {
            _uploadService = new UploadService(_outputPath);

            _fileBlobs = new List<FileBlob>()
            {
                new FileBlob() {Id = Guid.NewGuid(), Name = "FileBlob1", Path = "Path1", Size = 100},
                new FileBlob() {Id = Guid.NewGuid(), Name = "FileBlob2", Path = "Path2", Size = 100},
                new FileBlob() {Id = Guid.NewGuid(), Name = "FileBlob3", Path = "Path3", Size = 100}
            };

            _db.FileBlobs.AddRange(_fileBlobs);
            await _db.SaveChangesAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            _db.FileBlobs.RemoveRange(_fileBlobs);
            await _db.SaveChangesAsync();

            foreach (var key in _chunks.Keys)
            {
                if (File.Exists(_outputPath + key))
                    File.Delete(_outputPath + key);
            }
        }

        [Test]
        public async Task should_retrieve_file_blob_list()
        {
            (await _uploadService.GetFileBlobs()).Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task should_retrieve_file_blob()
        {
            var fileBlob = await _uploadService.GetFileBlob(_fileBlobs[0].Id);

            fileBlob.Should().NotBeNull();
            fileBlob.ShouldBeEquivalentTo(_fileBlobs[0]);
        }

        [Test]
        public async Task should_upload_one_chunk()
        {
            var key = _chunks.First().Key;
            var correlationId = _uploadService.StartNewSession();
            var result = await UploadChunk(key, _chunks[key][0], 1, _chunks[key].Length, correlationId);

            result.Should().BeFalse();
        }

        [Test]
        public async Task should_upload_all_chunks_of_one_file()
        {
            var correlationId = _uploadService.StartNewSession();
            var key = _chunks.First().Key;
            var tasks = new List<Task<bool>>();

            for (var i = 0; i < _chunks[key].Length; i++)
            {
                tasks.Add(UploadChunk(key, _chunks[key][i], i + 1, _chunks[key].Length, correlationId));
            }

            await Task.WhenAll(tasks);

            tasks.Where(t => t.Result).Should().NotBeNullOrEmpty();
            File.Exists(_outputPath + _chunks.First().Key).Should().BeTrue();
        }

        [Test]
        public void should_start_new_session()
        {
            var correlationId = _uploadService.StartNewSession();

            correlationId.Should().NotBeEmpty();
        }

        private async Task<bool> UploadChunk(string key, string filename, int chunkNumber, int totalChunks, Guid correlationId)
        {
            using (var stream = new FileStream(_filepath + filename, FileMode.Open))
            {
                var multiPartContent = new MultipartFormDataContent("boundary=---skj2vj42al0dk45vda");
                var streamContent = new StreamContent(stream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("file")
                {
                    FileName = key
                };

                multiPartContent.Add(streamContent);
                multiPartContent.Add(new StringContent(correlationId.ToString()), "correlationId");
                multiPartContent.Add(new StringContent(chunkNumber.ToString()), "chunkNumber");
                multiPartContent.Add(new StringContent(totalChunks.ToString()), "totalChunks");

                var request = new HttpRequestMessage
                {
                    Content = multiPartContent
                };

                return await _uploadService.UploadChunk(request);
            }
        }
    }
}
