using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CashForecasting.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilesController : Controller
    {
        private const string UploadContainerName = "datauploaded";
        private readonly AzureBlobManager _blobManager;

        public FilesController(AzureBlobManager blobManager)
        {
            _blobManager = blobManager;
        }

        [HttpPost("upload"), DisableRequestSizeLimit]
        public IActionResult Post()
        {
            if (Request?.Form?.Files == null || Request.Form.Files.Count == 0)
            {
                return BadRequest("No file provided.");
            }

            var file = Request.Form.Files[0];

            if (file.Length == 0)
            {
                return BadRequest("Uploaded file is empty.");
            }

            UploadBlob(file.FileName, file);
            return Ok("Upload successful.");
        }

        /// <summary>
        /// Uploads data to blob as memory stream.
        /// </summary>
        private static void UploadToBlobViaMemStream(string blobName, AzureBlobManager blobManager, IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                blobManager.PutBlob(blobManager.ContainerName, blobName, memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Uploads content to blob storage.
        /// </summary>
        private void UploadBlob(string sourceFileName, IFormFile file)
        {
            var blobName = GenerateNameFromFile(sourceFileName);
            _blobManager.ContainerName = UploadContainerName;
            UploadToBlobViaMemStream(blobName, _blobManager, file);
        }

        /// <summary>
        /// Prepend timestamp to uploaded file name.
        /// </summary>
        private static string GenerateNameFromFile(string sourceFileName)
        {
            var fileName = Path.GetFileName(sourceFileName);
            var currentTime = DateTime.UtcNow.ToString("yy-MM-dd-HH-mm-");
            return currentTime + fileName;
        }
    }
}
