using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CashForecasting.Api.Controllers
{
	[Route("[controller]")]
    [ApiController]
    public class FilesController : Controller
    {
		private readonly AzureBlobManager _blobManager;

		public FilesController(AzureBlobManager blobManager)
		{
			_blobManager = blobManager;
		}
		[HttpPost("upload"), DisableRequestSizeLimit]
		public IActionResult Post()
		{
			var file = Request.Form.Files[0];
			var fileName = Request.Form.Files[0].Name;

			UploadBlob(fileName, file);

			return Json("Upload Successful.");
		}

		/// <summary>
		/// Uploads data to blob as memory stream
		/// </summary>
		/// <param name="name">Name of the file from the POST request</param>
		/// <param name="newName">Returned name with current date pre-pended</param>
		/// <param name="abm">Reference to Azure Blob Manager</param>
		private static void UploadToBlobViaMemStream(string name, string newName, AzureBlobManager abm, IFormFile file)
		{
			// Convert IFormFile to B64 string and upload to blob storage		
			using (var ms = new MemoryStream())
			{
				file.CopyTo(ms);
				var fileBytes = ms.ToArray();
				string s      = Convert.ToBase64String(fileBytes);
				abm.PutBlob(abm.ContainerName, newName, fileBytes);
			}
		}


		/// <summary>
		/// Uploads content to blob storage
		/// </summary>
		/// <param name="name">Parsed filepath from URL in web request. Will be used to generate a name for the file stored in Blob.</param>
		private void UploadBlob(string name, IFormFile file)
		{
			string newName = GenerateNameFromFile(name);

			// Create blob storage client and assign the container to upload to
			//AzureBlobManager abm = new AzureBlobManager();
			//				 abm.ContainerName = "datauploaded";
			_blobManager.ContainerName = "datauploaded";
			//UploadToBlobViaMemStream(name, newName, abm, file);
			UploadToBlobViaMemStream(name, newName, _blobManager, file);
		}

		/// <summary>
		///  Grab file name from file & append the date
		/// </summary>
		/// <param name="name">Name from the file POSTed</param>
		private static string GenerateNameFromFile(string name)
		{
			// Get file name from the url
			string fileName    = Path.GetFileName(name);
			string currentTime = DateTime.Now.ToString("yy-M-dd-HH-mm-");
			string newName     = currentTime + fileName;

			return newName;
		}

	}
}
