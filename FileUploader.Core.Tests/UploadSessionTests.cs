using System;
using System.IO;
using System.Threading.Tasks;
using FileUploader.Core.Models;
using FluentAssertions;
using NUnit.Framework;

namespace FileUploader.Core.Tests
{
    [TestFixture]
    public class UploadSessionTests
    {
        private readonly string _filename = "Programming C#.pdf";
        private readonly string _filepath = $@"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}\files\";

        [Test]
        public void should_add_chunk_to_session()
        {
            var session = new UploadSession();
            session.AddChunk(_filename, "BodyPart_5e9c51db-180a-47c6-864c-256474957298", 1, 4);

            session.Chunks.Count.ShouldBeEquivalentTo(1);
        }

        [Test]
        public void should_add_chunk_metadata()
        {
            var session = new UploadSession();
            session.AddChunk(_filename, "BodyPart_5e9c51db-180a-47c6-864c-256474957298", 1, 4);

            session.Filename.Should().NotBeNullOrEmpty();
            session.Filename.Should().BeEquivalentTo(_filename);

            ChunkMetadata chunk;
            session.Chunks.TryPeek(out chunk);
            chunk.Filename.Should().NotBeNullOrEmpty();
            chunk.Filename.Should().BeEquivalentTo("BodyPart_5e9c51db-180a-47c6-864c-256474957298");
            chunk.ChunkNumber.Should().NotBe(null);
            chunk.ChunkNumber.ShouldBeEquivalentTo(1);
        }

        [Test]
        public void should_return_completed_when_the_last_chunk_is_added()
        {
            var session = new UploadSession();
            var completed = session.AddChunk(_filename, _filepath + "BodyPart_5e9c51db-180a-47c6-864c-256474957298", 1, 1);

            completed.Should().BeTrue();
        }

        [Test]
        public async Task should_merge_chunk_list()
        {
            var outputPath = @"c:\temp\chunks\";
            var fileName = "test.pdf";
            var session = new UploadSession();
            session.AddChunk(fileName, _filepath + "BodyPart_cfdf1acb-e4d0-48dd-8152-c73eff4c23cb", 1, 2);
            session.AddChunk(fileName, _filepath + "BodyPart_cfdf1acb-e4d0-48dd-8152-c73eff4c23cc", 2, 2);

            await session.MergeChunks(outputPath);

            using (var mainFile = new FileStream(outputPath + fileName, FileMode.Open))
            {
                mainFile.Length.ShouldBeEquivalentTo(254874);
            }
        }
    }
}
