using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly string _filename = "Programming C#.pdf";
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
        }

        [Test]
        public void should_retrieve_file_blob_list()
        {
            _uploadService.GetFileBlobs().Count.Should().BeGreaterThan(0);
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
            using (var stream = new FileStream(_filepath + _filename, FileMode.Open))
            {
                var multiPartContent = new MultipartFormDataContent("boundary=---skj2vj42al0dk45vda");
                var streamContent = new StreamContent(stream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("file")
                {
                    FileName = _filename
                };

                multiPartContent.Add(streamContent);
                multiPartContent.Add(new StringContent(_uploadService.StartNewSession().ToString()), "correlationId");
                multiPartContent.Add(new StringContent("1"), "chunkNumber");
                multiPartContent.Add(new StringContent("4"), "totalChunks");

                var request = new HttpRequestMessage
                {
                    Content = multiPartContent
                };

                var result = await _uploadService.UploadChunk(request);

                result.Should().BeFalse();
            }
        }

        [Test]
        public void should_start_new_session()
        {
            var correlationId = _uploadService.StartNewSession();

            correlationId.Should().NotBeEmpty();
        }
    }
}
