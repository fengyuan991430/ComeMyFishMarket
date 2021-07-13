using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Web;
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

        public bool UploadBlobImage(IFormFile files, string productname)
        {
            CloudBlobContainer container = getBlobContainerInformation();
            container.CreateIfNotExistsAsync();
            
            CloudBlockBlob blobitem = null;
            try
            {
                blobitem = container.GetBlockBlobReference(productname+files.FileName);
                if(Path.GetExtension(blobitem.Name) != ".jpg" && Path.GetExtension(blobitem.Name) != ".png" && Path.GetExtension(blobitem.Name) != ".jpeg")
                {                    
                    return false;                    
                }
                else
                {
                    var stream = files.OpenReadStream();
                    blobitem.UploadFromStreamAsync(stream).Wait();
                    return true;
                }                    
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<string> ViewBlobImage(string ProductImage)
        {
            CloudBlobContainer container = this.getBlobContainerInformation();
            List<string> bloblist = new List<string>();
            BlobResultSegment listing = container.ListBlobsSegmentedAsync(null).Result;

            foreach(IListBlobItem item in listing.Results)
            {
                if(item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    if(blob.Name.Equals(ProductImage))
                    {
                        bloblist.Add(blob.Name+"#"+blob.Uri);
                    }
                }
            }

            return bloblist;
        }
    }
}
