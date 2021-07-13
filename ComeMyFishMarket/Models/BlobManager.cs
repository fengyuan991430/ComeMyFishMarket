using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Models
{
    public class BlobManager
    {
        private CloudBlobContainer getBlobContainerInformation()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
            IConfiguration configure = builder.Build();

            //get the access connection string so that the app able to connect to the correct storage
            CloudStorageAccount accountdetails = CloudStorageAccount.Parse(configure["ConnectionStrings:blobstorageconnection"]);

            //Create client object to refer to correct container
            CloudBlobClient clientagent = accountdetails.CreateCloudBlobClient();
            CloudBlobContainer container = clientagent.GetContainerReference("cmfmcontainer");

            return container;
        }
        public void CreateContainer()
        {
            CloudBlobContainer container = getBlobContainerInformation();
            container.CreateIfNotExistsAsync();
        }

        public bool UploadBlobImage(List<IFormFile> files)
        {
            CloudBlobContainer container = getBlobContainerInformation();
            if(container == null)
            {
                container.CreateIfNotExistsAsync();
            }
            CloudBlockBlob blobitem = null;
            foreach(var file in files)
            {
                try
                {
                    blobitem = container.GetBlockBlobReference(file.FileName);
                    if(Path.GetExtension(blobitem.Name) != ".jpg" || Path.GetExtension(blobitem.Name) != ".png" || Path.GetExtension(blobitem.Name) != ".jpeg")
                    {
                        return false;
                    }
                    else
                    {
                        var stream = file.OpenReadStream();
                        blobitem.UploadFromStreamAsync(stream).Wait();
                        return true;
                    }
                    
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
