//C# Azure Blob Storage Manager class. 
//Basic functionality for blob storage in the Azure cloud.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CashForecasting.Api
{
	#region Sample Use
	/********************************************************************************
     * Code adapted from
     * http://dotnetspeak.com/2012/08/uploading-directory-to-azure-blob-storage
     * 
     * Sample usage:

        AzureBlobManager abm = new AzureBlobManager();
        abm.ContainerName = AzureBlobManager.ROOT_CONTAINER_NAME;
        abm.DirectoryName = "TheBlob" + "/" + "PathYouWant" + "/";

        //Check if the Container Exists
        if (abm.DoesContainerExist(abm.ContainerName))
        {
            //If so, do this if you want to delete all the files already there.
            if (abm.DoesBlobDirectoryExist(abm.ContainerName, abm.DirectoryName))
            {
                abm.DeleteBlobDirectory(abm.ContainerName, abm.DirectoryName);
            }
        }
        else
        {
            throw new Exception("The specified Azure Container \"" + abm.ContainerName + "\" doesn't exist.");
        } 

        //For example purposes, using a List of object arrays which holds data from a database
        List<object[]> SQLDataByCols = new List<object[]>();

        // :    
        //TODO: populate SQLDataByCols with data
        // :

        //Upload multiple blobs in Parallel
        Parallel.For(0, SQLDataByCols.Count, x =>
        //foreach (object[] Col in SQLDataByCols)   //use this if you want it to be single threaded instead
        {
            object[] Col = SQLDataByCols[x];
            Guid guid = Guid.NewGuid();

            //Next line uploads the data, compressed, with just a Guid for the file name.
            //You can obviously use whatever name you want.
            //For cloud security purposes, it's a lot harder to figure out what a file is for if it's given a GUID as a name.
            //You don't have to compress your data. Just so long as it's passed as a byte[].
            //Just remember to decompress when you download again.
            abm.PutBlob(abm.ContainerName, abm.DirectoryName + guid + ".dat", Utilities.Zip(JsonConvert.SerializeObject(Col)));

        }   //for loop
        );  //parallel

     * 
     *
    *********************************************************************************/
	#endregion  Sample Use

	public class AzureBlobManagerOptions
	{
		public string ConnectionString { get; set; }
	}

	/// <summary>
	/// Blob storage manager class. Basic functionality for blob storage in the Azure cloud.
	/// </summary>
	public class AzureBlobManager
	{
		/// <summary>
		/// The root top level container in Azure blob.
		/// Rename as appropriate for your system.
		/// NOTE: MUST be --lower case--, otherwise Azure returns a 400 error
		/// </summary>
		public const string ROOT_CONTAINER_NAME = "datauploaded";
		public string status_SUCCESS = "Sucessfully uploaded doc to Azure";

		private CloudBlobClient _blobClient;
		private string _containerName;
		private string _blobName;
		private string _directoryName;

		// Used with appsettings.json
		private static string connString = ConfigurationManager.AppSettings["ConnString"];
		private static string _key = ConfigurationManager.AppSettings["Key"];
		private static string _name = ConfigurationManager.AppSettings["AccountName"];
		//private static string myConfig =   Configuration["Authentication:Microsoft:ApplicationId"];


		//To initialize the default storage credentials if none are provided. 
		//For now we're going to assume everything is going to this blob storage.
		//private StorageCredentials _storageCredentials = new StorageCredentials(_name, _key);
		//private string connStr = ConfigurationManager.AppSettings["Values"];
		private static string connStr = Environment.GetEnvironmentVariable("ConnString");

		private CloudStorageAccount _storageAccount;// = new CloudStorageAccount(_storageCredentials, false);
		private CloudBlobContainer _container;
		private CloudBlockBlob _blockBlob;     // = _container.GetBlockBlobReference("myfirstupload.txt");


		#region Constructors

		public AzureBlobManager(IOptions<AzureBlobManagerOptions> options)
		{

			var connString = options.Value.ConnectionString;

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AzureBlobManager" /> class 
		/// with the default values for the storage credentials
		/// and the associated containername as specified by the private members.
		/// </summary>
		public AzureBlobManager(IConfiguration configuration)
		{
			var connString = configuration["Values:ConnString"];

			//_storageAccount   = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=forcastdatastracc;AccountKey=RaVm+Htv2C6R7OvPU/9kf5oU1o7WWh/HUZBIBzNwaK6WqHpYHEdzT9X44lk/A6C41DjIJlSzbeF6LHFVjXdsfA==;EndpointSuffix=core.windows.net");
			_storageAccount = CloudStorageAccount.Parse(connString);
			_blobClient = _storageAccount.CreateCloudBlobClient();
			_container = _blobClient.GetContainerReference(ROOT_CONTAINER_NAME);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AzureBlobManager" /> class 
		/// with the specified container and storage credentials.
		/// </summary>
		public AzureBlobManager(string containerName)
		{
			_storageAccount = CloudStorageAccount.Parse(connString);
			_blobClient = _storageAccount.CreateCloudBlobClient();
			_containerName = containerName;
			_container = _blobClient.GetContainerReference(_containerName);
		}

		#endregion Constructors

		#region Public Properties

		/// <summary>
		/// Gets/sets the name of the Azure Container
		/// </summary>
		public string ContainerName
		{
			get
			{
				return _containerName;
			}
			set
			{
				_containerName = value;
			}
		}

		/// <summary>
		/// Gets/sets the name of the Blob
		/// </summary>
		public string BlobName
		{
			get
			{
				return _blobName;
			}
			set
			{
				_blobName = value;
			}
		}

		/// <summary>
		/// Gets/sets the "virtual" directory. 
		/// </summary>
		public string DirectoryName
		{
			get
			{
				return _directoryName;
			}
			set
			{
				_directoryName = value;
			}
		}

		#endregion Public Properties

		#region Public Methods

		//public void HandleStream()
		//{
		//    var sContainerName = AppendDateToName(ROOT_CONTAINER_NAME);
		//    if (DoesContainerExist(sContainerName) == true)
		//    {
		//        var container = _blobClient.GetContainerReference(sContainerName);

		//    }
		//    else
		//    {

		//    }
		//    var container = _blobClient.GetContainerReference(sContainerName);
		//}

		/// <summary>
		/// Loops through blobs in a blob container.
		/// </summary>
		/// <param name="containerName">Container to parse blobs from</param>
		/// <returns>List of Cloud Blobs</returns>
		public List<CloudBlob> GetAllBlobsInContainerAsCloudBlob(string containerName)
		{
			var listOfBlobs = new List<CloudBlob>();
			var container = _blobClient.GetContainerReference(containerName);
			var blobs = container.ListBlobs(useFlatBlobListing: true);

			foreach (var item in blobs)
			{
				var blob = (CloudBlockBlob)item;
				listOfBlobs.Add(blob);
			}
			return listOfBlobs;
		}




		/// <summary>
		/// Retrieves all blob names from within a container.
		/// </summary>
		/// <param name="containerName">Container to retrieve blobs from.</param>
		/// <returns>IEnumerable string of the blob names</returns>
		public IEnumerable<string> GetAllBlobNames(string containerName)
		{
			try
			{
				CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
				var blobs = container.ListBlobs(useFlatBlobListing: true);
				var blobNames = new List<string>();

				foreach (var item in blobs)
				{
					var blob = (CloudBlockBlob)item;
					blob.FetchAttributes();
					blobNames.Add(blob.Name);
				}
				return blobNames;
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't get all blob names" + ex);
			}
		}


		/// <summary>
		/// Updates or created a blob in Azure blob storage
		/// </summary>
		/// <param name="containerName">Name of the container to upload into.</param>
		/// <param name="blobName">Name of the blob.</param>
		/// <param name="content">The content of the blob.</param>
		/// <returns></returns>
		public bool PutBlob(string containerName, string blobName, byte[] content)
		{
			return ExecuteWithExceptionHandlingAndReturnValue(
					() =>
					{
						CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
						_blockBlob = container.GetBlockBlobReference(blobName);
						using (var stream = new MemoryStream(content, writable: false))
						{
							_blockBlob.UploadFromStream(stream);
						}
					});
		}


		/// <summary>
		/// Updates or created a blob in Azure blob storage via a byte array
		/// </summary>
		/// <param name="containerName">Name of the container to upload into.</param>
		/// <param name="blobName">Name of the blob.</param>
		/// <param name="content">The content of the blob.</param>
		/// <returns></returns>
		public bool PutBlobAsByteArray(string containerName, string blobName, byte[] content)
		{
			return ExecuteWithExceptionHandlingAndReturnValue(
					() =>
					{
						CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
						_blockBlob = container.GetBlockBlobReference(blobName);
						_blockBlob.UploadFromByteArray(content, 0, content.Length);
					});
		}

		/// <summary>
		/// Creates the container in Azure blob storage. 
		/// This really shouldn't need to happen once a system has been established.
		/// Should call DoesContainerExist to see if it exists before calling this method.
		/// </summary>
		/// <param name="containerName">Name of the container.</param>
		/// <returns>True if container was created successfully; false otherwise</returns>
		public bool CreateContainer(string containerName)
		{
			return ExecuteWithExceptionHandlingAndReturnValue(
					() =>
					{
						CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
						container.Create();
					});
		}

		/// <summary>
		/// Deletes the container in Azure blob storage. 
		/// Better make sure you have backups before you call this. :-)
		/// </summary>
		/// <param name="containerName">Name of the container.</param>
		/// <returns>True if contianer was deleted successfully; false otherwise.</returns>
		public bool DeleteContainer(string containerName)
		{
			return ExecuteWithExceptionHandlingAndReturnValue(
					() =>
					{
						CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
						container.Delete();
					});
		}

		/// <summary>
		/// Checks if a container exists.
		/// </summary>
		/// <param name="containerName">Name of the container.</param>
		/// <returns>True if container exists; false otherwise</returns>
		public bool DoesContainerExist(string containerName)
		{
			bool returnValue = false;
			ExecuteWithExceptionHandling(
					() =>
					{
						IEnumerable<CloudBlobContainer> containers = _blobClient.ListContainers();
						returnValue = containers.Any(one => one.Name == containerName);
					});
			return returnValue;
		}

		/// <summary>
		/// Checks if a blob exists.
		/// </summary>
		/// <param name="containerName">Name of the container.</param>
		/// <param name="blobName">Name of the blob.</param>
		/// <returns>True if blob exists; false otherwise</returns>
		public bool DoesBlobExist(string containerName, string blobName)
		{
			bool returnValue = false;
			ExecuteWithExceptionHandling(
					() =>
					{
						CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
						_blockBlob = container.GetBlockBlobReference(blobName);
						returnValue = _blockBlob.Exists();
					});
			return returnValue;
		}

		/// <summary>
		/// Checks if a blob (virtual) directory exists.
		/// </summary>
		/// <param name="containerName">Name of the container.</param>
		/// <param name="directoryName">Name of the directory.</param>
		/// <returns>True if the directory exists; false otherwise.</returns>
		public bool DoesBlobDirectoryExist(string containerName, string directoryName)
		{
			bool returnValue = false;
			ExecuteWithExceptionHandling(
					() =>
					{
						CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
						returnValue = container.ListBlobs(directoryName, false).ToArray().Length > 0;
					});
			return returnValue;
		}

		/// <summary>
		/// Moves a Blob directory from one location to another within the same container. 
		/// If the target directory exists prior to the copy, it will be deleted first. 
		/// </summary>
		/// <param name="containerName">The blob container to move to. If the container does not exist it will be created.</param>
		/// <param name="sourceDirectory">The source blob directory to copy the blobs from. Should end with a "/" character.</param>
		/// <param name="targetDirectory">The target blob directory to copy the blobs to. Should end with a "/" character. If it
		/// already exists, it will be deleted, along with any blobs already contained within.</param>
		/// <returns>true indicating success; false otherwise.</returns>
		public bool MoveBlobDirectory(string containerName, string sourceDirectory, string targetDirectory)
		{
			bool returnValue = true;
			ExecuteWithExceptionHandling(
				   () =>
				   {
					   //Check the container exists. If not, create it.
					   if (!DoesContainerExist(containerName))
					   {
						   CreateContainer(containerName);
					   }

					   CloudBlobContainer container = _blobClient.GetContainerReference(containerName);

					   //see if the target directory exists
					   if (DoesBlobDirectoryExist(containerName, targetDirectory))
					   {
						   //if so, see if we can delete it
						   if (!DeleteBlobDirectory(containerName, targetDirectory))
							   returnValue = false;
					   }

					   if (returnValue)  //no issues thus far. That's good. 
					   {
						   //copy the files over in parallel
						   Parallel.ForEach(container.ListBlobs(sourceDirectory, false), item =>
						   //foreach (IListBlobItem item in container.ListBlobs(sourceDirectory, false))
						   {
							   //These are declared inside the loop for multi-threading purposes.
							   string sourceBlobName = string.Empty;
							   string targetBlobName = string.Empty;
							   CloudBlockBlob sourceBlob;
							   CloudBlockBlob targetBlob;

							   try
							   {
								   if (item.GetType() == typeof(CloudBlob) || item.GetType().BaseType == typeof(CloudBlob))
								   {
									   sourceBlobName = ((CloudBlob)item).Name;
									   sourceBlob = container.GetBlockBlobReference(sourceBlobName);

									   //Set the new blob name, replacing the prefix with an 
									   //empty string to just get the name of the file.
									   //The prefix contains the virtual folder path.
									   targetBlobName = ((CloudBlob)item).Name.Replace(((CloudBlob)item).Parent.Prefix, String.Empty);

									   targetBlob = container.GetBlockBlobReference(targetDirectory + targetBlobName);
									   targetBlob.StartCopy(sourceBlob);
									   DeleteBlob(containerName, sourceBlobName);
								   }
							   }
							   catch (Exception e)
							   {
								   //Something happened, so assume not successful.
								   returnValue = false;
							   }
						   }    //for loop
							   );   //parallel
					   }
				   });
			return returnValue;
		}


		/// <summary>
		/// Deletes the specified blob from the specified container if it exists.
		/// </summary>
		/// <param name="containerName">The name of the Container</param>
		/// <param name="blobName">The blob "file" to delete.</param>
		/// <returns></returns>
		public bool DeleteBlob(string containerName, string blobName)
		{
			return ExecuteWithExceptionHandlingAndReturnValue(
					() =>
					{
						CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
						_blockBlob = container.GetBlockBlobReference(blobName);
						_blockBlob.Delete();
					});
		}

		/// <summary>
		/// Removes a "virtual" blob directory. For a directory to be removed, 
		/// every single blob file must also be removed. 
		/// So this will remove all the blobs within this directory. Make sure that's what you want!
		/// </summary>
		/// <param name="containerName">The container containing the directory.</param>
		/// <param name="directoryName">The virtual directory to remove.</param>
		/// <returns></returns>
		public bool DeleteBlobDirectory(string containerName, string directoryName)
		{
			return ExecuteWithExceptionHandlingAndReturnValue(
					() =>
					{
						CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
						Parallel.ForEach(container.ListBlobs(directoryName, false), item =>
						//foreach (IListBlobItem item in container.ListBlobs(directoryName,false))
						{
							if (item.GetType() == typeof(CloudBlob) || item.GetType().BaseType == typeof(CloudBlob))
							{
								((CloudBlob)item).Delete();
							}
						}
						);
					});
			//return true;
		}

		/// <summary>
		/// Uploads the directory to blobl storage
		/// </summary>
		/// <param name="sourceDirectory">The source directory name.</param>
		/// <param name="containerName">Name of the container to upload to.</param>
		/// <returns>True if successfully uploaded</returns>
		public bool UploadDirectory(string sourceDirectory, string containerName)
		{
			return UploadDirectory(sourceDirectory, containerName, string.Empty);
		}

		/// <summary>
		/// Recursive function to upload files in a directory to an Azure blob directory. 
		/// HASN'T BEEN FULLY TESTED.
		/// </summary>
		/// <param name="sourceDirectory">The directory containing all the files to be copied</param>
		/// <param name="containerName">The target Azure container</param>
		/// <param name="prefixAzureFolderName">The destination "folder" to copy everything to in Azure.</param>
		/// <returns>true indicating success; false otherwise.</returns>
		private bool UploadDirectory(string sourceDirectory, string containerName, string prefixAzureFolderName)
		{
			return ExecuteWithExceptionHandlingAndReturnValue(
				() =>
				{
					if (!DoesContainerExist(containerName))
					{
						CreateContainer(containerName);
					}

					var folder = new DirectoryInfo(sourceDirectory);
					FileInfo[] files = folder.GetFiles();
					string blobName = String.Empty;
					foreach (var fileInfo in files)
					{
						blobName = fileInfo.Name;
						if (!string.IsNullOrEmpty(prefixAzureFolderName))
						{
							blobName = prefixAzureFolderName + "/" + blobName;
						}
						PutBlob(containerName, blobName, File.ReadAllBytes(fileInfo.FullName));
					}

					DirectoryInfo[] subFolders = folder.GetDirectories();
					string prefix = String.Empty;
					foreach (var directoryInfo in subFolders)
					{
						prefix = directoryInfo.Name;
						if (!string.IsNullOrEmpty(prefixAzureFolderName))
						{
							prefix = prefixAzureFolderName + "/" + prefix;
						}
						UploadDirectory(directoryInfo.FullName, containerName, prefix);
					}
				});
		}


		/// <summary>
		///  Appends current date to file name with format: yy-MM-dd
		///  Later on we'll use this to search for containers within a week and return those w/ images
		/// </summary>
		/// <returns>A string with current date to file name with format: yy-MM-dd</returns>
		private static string AppendDateToName(string sRootName)
		{
			string currentDate = DateTime.Now.ToString("-yy-MM-dd");
			string newName = sRootName + currentDate;

			return newName;
		}

		/// <summary>
		/// Prepends current date to file name with format: yy-MM-dd-HH-mm-ss
		/// </summary>
		/// <param name="sRootName">Name of file we are prepending</param>
		/// <returns>A string with current date to file name with format: yy-MM-dd-HH-mm-ss</returns>
		private static string PrependDateToNameJpg(string sRootName)
		{
			string currentDate = DateTime.Now.ToString("yy-MM-dd-HH-mm-ss-");
			string newName = currentDate + sRootName + ".jpg";

			return newName;
		}

		#endregion  Public Methods

		#region Private Methods

		//A good read on storage exceptions:
		//http://stackoverflow.com/questions/15623306/error-handling-for-windows-azure-storage-migration-from-1-7-to-2-0
		//and
		//https://alexandrebrisebois.wordpress.com/2013/07/03/handling-windows-azure-storage-exceptions/
		/// <summary>
		/// Tries the perform the Action. If an exception occurs that is not a "409" error, the
		/// exception will be rethrown. Blob service error codes: https://msdn.microsoft.com/en-au/library/azure/dd179439.aspx
		/// </summary>
		/// <param name="action">The Action to perform.</param>
		private void ExecuteWithExceptionHandling(Action action)
		{
			try
			{
				action();
			}
			catch (StorageException ex)
			{
				//Blob service error codes: https://msdn.microsoft.com/en-au/library/azure/dd179439.aspx
				//Ignore lease already present error
				if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode != "409")
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Tries to perform the specified Action. An exception is thrown that is not a "409" exception.
		/// Blob service error codes: https://msdn.microsoft.com/en-au/library/azure/dd179439.aspx
		/// </summary>
		/// <param name="action">The Action to perform.</param>
		/// <returns>True if no exceptions or only a 409 error; false otherwise.</returns>
		private bool ExecuteWithExceptionHandlingAndReturnValue(Action action)
		{
			try
			{
				action();
				return true;
			}
			catch (StorageException ex)
			{
				//Blob service error codes: https://msdn.microsoft.com/en-au/library/azure/dd179439.aspx
				//Ignore lease already present error
				if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode != "409")
				{
					return false;
				}
				throw;
			}
		}


		/// <summary>
		/// Downloads the contents of a blob into a byte[]
		/// </summary>
		/// <param name="containerName">The blob's container</param>
		/// <param name="blobName">The name of the blob to download</param>
		/// <returns>Byte[] with the blob's contents</returns>
		private byte[] GetBlobAsByteArr(string containerName, string blobName)
		{
			CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
			_blockBlob = container.GetBlockBlobReference(blobName);
			_blockBlob.FetchAttributes();

			long fileByteLength = _blockBlob.Properties.Length;
			byte[] fileContentsAsByteArr = new byte[fileByteLength];
			_blockBlob.DownloadToByteArray(fileContentsAsByteArr, 0);

			return fileContentsAsByteArr;
		}


		/// <summary>
		/// Save blob as a Byte Array.
		/// </summary>
		/// <param name="blob">Blob to be converted to byte array</param>
		/// <returns>Byte Array of blob from Azure</returns>
		private byte[] GetBlobAsByteArr(CloudBlob blob)
		{
			blob.FetchAttributes();
			long fileByteLength = blob.Properties.Length;
			byte[] fileContentsAsByteArr = new byte[fileByteLength];
			blob.DownloadToByteArray(fileContentsAsByteArr, 0);

			return fileContentsAsByteArr;
		}

		#endregion  Private Methods
	}
}

