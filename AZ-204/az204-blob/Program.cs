using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace az204_blob
{
    class Program
    {
        const string LocalPath = "./data/";

        public static async Task Main()
        {
            Console.WriteLine("Azure Blob Storage exercise\n");

            // Run the examples asynchronously, wait for the results before proceeding
            await ProcessAsync();

            Console.WriteLine("Press enter to exit the sample application.");
            Console.ReadLine();

        }

        private static async Task ProcessAsync()
        {
            // Copy the connection string from the portal in the variable below.
            var uri = new Uri("https://euanmctest.blob.core.windows.net/");

            // Create a client that can authenticate with a connection string
            BlobServiceClient blobServiceClient = new BlobServiceClient(uri, new DefaultAzureCredential());

            BlobContainerClient containerClient = await CreateContainer(blobServiceClient);

            BlobClient blobClient = await UploadBlob(containerClient);

            await ListBlobs(containerClient);

            await DownloadBlob(blobClient);

            await DeleteContainer(containerClient);
        }

        private static async Task<BlobContainerClient> CreateContainer(BlobServiceClient blobServiceClient)
        {
            //Create a unique name for the container
            string containerName = "blob-" + Guid.NewGuid().ToString();

            // Create the container and return a container client object
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
            Console.WriteLine("A container named '" + containerName + "' has been created. " +
                "\nTake a minute and verify in the portal." +
                "\nNext a file will be created and uploaded to the container.");
            Console.WriteLine("Press 'Enter' to continue.");
            Console.ReadLine();
            return containerClient;
        }

        private static async Task<BlobClient> UploadBlob(BlobContainerClient containerClient)
        {
            string fileName = "file-" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = Path.Combine(LocalPath, fileName);

            // Write text to the file
            await File.WriteAllTextAsync(localFilePath, "Hello, World!");

            // Get a reference to the blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

            // Open the file and upload its data
            using FileStream uploadFileStream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();

            Console.WriteLine("\nThe file was uploaded. We'll verify by listing" +
                    " the blobs next.");
            Console.WriteLine("Press 'Enter' to continue.");
            Console.ReadLine();
            return blobClient;
        }

        private static async Task ListBlobs(BlobContainerClient containerClient)
        {
            // List blobs in the container
            Console.WriteLine("Listing blobs...");
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }

            Console.WriteLine("\nYou can also verify by looking inside the " +
                    "container in the portal." +
                    "\nNext the blob will be downloaded with an altered file name.");
            Console.WriteLine("Press 'Enter' to continue.");
            Console.ReadLine();
        }

        private static async Task DownloadBlob(BlobClient blobClient)
        {
            string downloadFile = blobClient.Name + "_DOWNLOADED.txt";
            string downloadFilePath = Path.Combine(LocalPath, downloadFile);
            Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
            {
                await download.Content.CopyToAsync(downloadFileStream);
                downloadFileStream.Close();
            }
            Console.WriteLine("\nLocate the local file to verify it was downloaded.");
            Console.WriteLine("The next step is to delete the container and local files.");
            Console.WriteLine("Press 'Enter' to continue.");
            Console.ReadLine();
        }

        private static async Task DeleteContainer(BlobContainerClient containerClient)
        {
            // Delete the container and clean up local files created
            Console.WriteLine("\n\nDeleting blob container...");
            await containerClient.DeleteAsync();

            Console.WriteLine("Deleting the local source and downloaded files...");
            DirectoryInfo di = new DirectoryInfo(LocalPath);
            foreach (FileInfo file in di.GetFiles())
                file.Delete();
            foreach (DirectoryInfo dir in di.GetDirectories())
                dir.Delete(true);

            Console.WriteLine("Finished cleaning up.");
        }
    }
}
