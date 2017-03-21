using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using FileUploader.Core.DataLayer;
using FileUploader.Core.ServiceLayer;

namespace FileUploader.Web.Controllers.Api
{
    public class FileBlobsController : ApiController
    {
        private readonly IUploadService _fileBlobsService;
        private readonly Context _db = new Context();

        public FileBlobsController(IUploadService uploadService)
        {
            _fileBlobsService = uploadService;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            return Ok(await _fileBlobsService.GetFileBlobs());
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            var fileBlob = await _fileBlobsService.GetFileBlob(id);

            using (var stream = new FileStream(fileBlob.Path, FileMode.Open))
            {
                byte[] content = new byte[stream.Length];
                await stream.ReadAsync(content, 0, int.Parse(stream.Length.ToString()));

                var result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new MemoryStream(content));
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = fileBlob.Name;

                return result;
            }
        }

        [Route("api/fileblobs/getcorrelationid")]
        [HttpGet]
        public IHttpActionResult GetCorrelationId()
        {
            return Ok(_fileBlobsService.StartNewSession());
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostFileBlob()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new Exception();

            var result = await _fileBlobsService.UploadChunk(Request);

            return Ok(result);
        }
    }
}
