using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;

namespace BccAzureFunctions
{
    public static class FunctionPostBlob
    {
        [FunctionName("PostBlob")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "postblob/{mode}/{type}/{ext}")] HttpRequest req, ILogger log, string mode, string type, string ext)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Account account = new Account();
            account.mode = mode;
            account.type = type;
            account.extension = ext;
            account.objlen = requestBody.Length;

            if (requestBody.Length > 0)
            {
                account.method = "postblob";
                try
                {
                    BlobTransferAsync(account.getObjName(), requestBody, account);
                    if (account.fatalException.Length > 0)
                        return new BadRequestObjectResult(JsonConvert.SerializeObject(account, Formatting.Indented));
                } catch (Exception e)
                {
                    account.fatalException = e.ToString();
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(account, Formatting.Indented));
                }
            }
            string json = JsonConvert.SerializeObject(account, Formatting.Indented);
            ObjectResult resp = new OkObjectResult(json);
            return resp;
        }

        public static async void BlobTransferAsync(string azurename, string requestBody, Account account)
        {
            string acctName  = System.Environment.GetEnvironmentVariable("AcctName", EnvironmentVariableTarget.Process);
            string acctKey   = System.Environment.GetEnvironmentVariable("AcctKey", EnvironmentVariableTarget.Process);
            string container = System.Environment.GetEnvironmentVariable("Container", EnvironmentVariableTarget.Process);
            string directory = System.Environment.GetEnvironmentVariable("Directory", EnvironmentVariableTarget.Process);
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName={acctName};AccountKey={acctKey}";
            try { 
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(container);
                await cloudBlobContainer.CreateIfNotExistsAsync();

                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(azurename);
                blob.StreamWriteSizeInBytes = 256 * 1024; //256 k
                await blob.UploadTextAsync(requestBody);
            }
            catch (Exception e)
            {
                account.fatalException = e.Message;
            }
        }
    }
    public static class FunctionPostFile
    {
        [FunctionName("PostFile")]
        public static async Task<IActionResult> CreateBlob(
       [HttpTrigger(AuthorizationLevel.Function, "post", Route = "postfile/{mode}/{type}/{ext}")]HttpRequest req,
       //[Blob("bccGrayTech", Connection = "AzureWebJobsStorage")] CloudBlobContainer bccGraytechContainer,
       ILogger log, string mode, string type, string ext)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Account account = new Account();
            account.mode = mode;
            account.type = type;
            account.extension = ext;
            account.objlen = requestBody.Length;

            if (requestBody.Length > 0)
            {
                account.method = "postfile";
                try { 
                    FileTransferAsync(account.getObjName(), requestBody, account);
                    if (account.fatalException.Length>0)
                        return new BadRequestObjectResult(JsonConvert.SerializeObject(account, Formatting.Indented));
                } catch (Exception e)
                {
                    account.fatalException = e.ToString();
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(account, Formatting.Indented));
                }
        }
            string json = JsonConvert.SerializeObject(account, Formatting.Indented);
            ObjectResult resp = new OkObjectResult(json);
            return resp;
        }
        public static async void FileTransferAsync(string azurename, string requestBody, Account account)
        {
                string acctName  = System.Environment.GetEnvironmentVariable("AcctName", EnvironmentVariableTarget.Process);
                string acctKey   = System.Environment.GetEnvironmentVariable("AcctKey", EnvironmentVariableTarget.Process);
                string container = System.Environment.GetEnvironmentVariable("Container", EnvironmentVariableTarget.Process);
                string directory = System.Environment.GetEnvironmentVariable("Directory", EnvironmentVariableTarget.Process);
                string connectionString = $"DefaultEndpointsProtocol=https;AccountName={acctName};AccountKey={acctKey}";
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                CloudFileClient cloudFileClient = cloudStorageAccount.CreateCloudFileClient();
                CloudFileShare cloudFileShare = cloudFileClient.GetShareReference(container);
                Boolean done = await cloudFileShare.CreateIfNotExistsAsync();
                CloudFileDirectory rootDir = cloudFileShare.GetRootDirectoryReference();
                CloudFileDirectory azureDir = rootDir.GetDirectoryReference(directory);
                await azureDir.CreateIfNotExistsAsync();
                CloudFile cloudFile = azureDir.GetFileReference(azurename);
                await cloudFile.UploadTextAsync(requestBody);
            }
            catch (Exception e)
            {
                account.fatalException = e.Message;
            }

        }
    }

    public class Account {
        public string mode = "";
        public string type = "";
        public string method = "";
        public string extension = "";
        public string objname = "";
        public int objlen = 0;
        public DateTime dateTime = DateTime.Now;
        public string fatalException = "";

        public string getObjName()
        {
            string name = "azure." + mode + "." + type + "."+method+"." + dateTime.ToString("yyyyMMddHHmmss") + "." + extension;
            name = name.Replace("..", ".");
            objname = name;
            return name;
        }
    }
}
