using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FileUploader.Core.Providers
{
    public interface ICustomMultipartFormDataStreamProvider
    {
        string ChunkFilename { get; }
        int ChunkNumber { get; }
        string CorrelationId { get; }
        string Filename { get; }
        int TotalChunks { get; }
        void ExtractValues();
    }

    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider, ICustomMultipartFormDataStreamProvider
    {
        public string Filename { get; private set; }
        public string ChunkFilename { get; private set; }
        public string CorrelationId { get; private set; }
        public int ChunkNumber { get; private set; }
        public int TotalChunks { get; private set; }
        
        public CustomMultipartFormDataStreamProvider(string rootPath) : base(rootPath) { }

        public CustomMultipartFormDataStreamProvider(string rootPath, int bufferSize) : base(rootPath, bufferSize) { }

        public override Task ExecutePostProcessingAsync()
        {
            foreach (var file in Contents)
            {                
                var parameters = file.Headers.ContentDisposition.Parameters;
                var filename = ExtractParameter(parameters, "filename");
                if (filename != null) Filename = filename.Value.Trim('\"');
            }

            return base.ExecutePostProcessingAsync();
        }

        public void ExtractValues()
        {
            var chunkFileName = FileData[0].LocalFileName;
            var correlationId = FormData?.GetValues("correlationId");
            var chunkNumber = FormData?.GetValues("chunkNumber");
            var totalChunks = FormData?.GetValues("totalChunks");

            if (string.IsNullOrEmpty(chunkFileName) || correlationId == null || chunkNumber == null || totalChunks == null)
                throw new Exception("Missing values in UploadChunk session.");

            ChunkFilename = chunkFileName;
            CorrelationId = correlationId.First();
            ChunkNumber = int.Parse(chunkNumber.First());
            TotalChunks = int.Parse(totalChunks.First());
        }

        private NameValueHeaderValue ExtractParameter(ICollection<NameValueHeaderValue> parameters, string name)
        {
            return parameters.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
