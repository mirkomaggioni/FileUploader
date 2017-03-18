using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FileUploader.Core.Providers;
using FluentAssertions;
using NUnit.Framework;

namespace FileUploader.Core.Tests
{
    [TestFixture]
    public class CustomMultipartFormDataStreamProviderTests
    {
        private CustomMultipartFormDataStreamProvider _provider;
        private readonly string _filename = "Programming C#.pdf";
        private readonly string _filepath = $@"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}\files\";

        [SetUp]
        public void Setup()
        {
            _provider = new CustomMultipartFormDataStreamProvider(@"c:\temp\chunks\");
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public async Task should_extract_filename()
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

                var request = new HttpRequestMessage
                {
                    Content = multiPartContent
                };

                await request.Content.ReadAsMultipartAsync(_provider);

                _provider.Filename.Should().NotBeNullOrEmpty();
                _provider.Filename.ShouldBeEquivalentTo(_filename);
            }
        }

        [Test]
        public async Task should_extract_formdata_values()
        {
            var chunkFileName = _filepath + _filename;

            using (var stream = new FileStream(chunkFileName, FileMode.Open))
            {
                var multiPartContent = new MultipartFormDataContent("boundary=---skj2vj42al0dk45vda");
                var streamContent = new StreamContent(stream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("file")
                {
                    FileName = _filename
                };

                multiPartContent.Add(streamContent);
                multiPartContent.Add(new StringContent("1234"), "correlationId");
                multiPartContent.Add(new StringContent("1"), "chunkNumber");
                multiPartContent.Add(new StringContent("4"), "totalChunks");

                var request = new HttpRequestMessage
                {
                    Content = multiPartContent
                };

                await request.Content.ReadAsMultipartAsync(_provider);
                _provider.ExtractValues();

                _provider.ChunkFilename.Should().NotBeNullOrEmpty();
                _provider.CorrelationId.Should().NotBeNullOrEmpty();
                _provider.CorrelationId.ShouldBeEquivalentTo("1234");
                _provider.ChunkNumber.Should().NotBe(null);
                _provider.ChunkNumber.ShouldBeEquivalentTo("1");
                _provider.TotalChunks.Should().NotBe(null);
                _provider.TotalChunks.ShouldBeEquivalentTo("4");
            }
        }
    }
}
