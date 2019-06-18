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
<<<<<<< HEAD
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "postblob/{mode}/{senderid}/{doctype}/{ext}")] HttpRequest req,
            ILogger log, string mode, string senderId, string docType, string ext)
=======
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post"
            , Route = "postblob/{mode}/{senderid}/{doctype}/{ext}")] HttpRequest req
            , ILogger log, string mode, string senderId, string docType, string ext)
>>>>>>> develop
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Account account = new Account();
            account.mode = mode;
            account.senderId = senderId;
            account.docType = docType;
            account.ext = ext;
            account.method = "postblob";

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            account.objLen = requestBody.Length;

            if (requestBody.Length > 0)
            {
                try
                {
                    BlobTransferAsync(account.getObjName(), requestBody, account);
                    if (account.fatalException.Length > 0)
                        return new BadRequestObjectResult(JsonConvert.SerializeObject(account, Formatting.Indented));
                }
                catch (Exception e)
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
            string acctName = System.Environment.GetEnvironmentVariable("AcctName", EnvironmentVariableTarget.Process);
            string acctKey = System.Environment.GetEnvironmentVariable("AcctKey", EnvironmentVariableTarget.Process);
            string container = System.Environment.GetEnvironmentVariable("Container", EnvironmentVariableTarget.Process);
            string directory = System.Environment.GetEnvironmentVariable("Directory", EnvironmentVariableTarget.Process);
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName={acctName};AccountKey={acctKey}";
            try
            {
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
<<<<<<< HEAD
       [HttpTrigger(AuthorizationLevel.Function, "post", Route = "postfile/{mode}/{senderid}/{doctype}/{ext}")]HttpRequest req,
       //[Blob("bccGrayTech", Connection = "AzureWebJobsStorage")] CloudBlobContainer bccGraytechContainer,
       ILogger log, string mode, string senderId, string docType, string ext)
=======
       [HttpTrigger(AuthorizationLevel.Function, "post"
            , Route = "postfile/{mode}/{senderid}/{doctype}/{ext}")]HttpRequest req
            //, [Blob("bccGrayTech", Connection = "AzureWebJobsStorage")] CloudBlobContainer bccGraytechContainer
            , ILogger log, string mode, string senderId, string docType, string ext)
>>>>>>> develop
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Account account = new Account();
            account.mode = mode;
            account.senderId = senderId;
            account.docType = docType;
            account.ext = ext;
            account.method = "postblob";
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            account.objLen = requestBody.Length;
            //account.hostname = System.Environment.GetEnvironmentVariable("HostName", EnvironmentVariableTarget.Process);

            if (requestBody.Length > 0)
            {
                account.method = "postfile";
                try
                {
                    FileTransferAsync(account.getObjName(), requestBody, account);
                    if (account.fatalException.Length > 0)
                        return new BadRequestObjectResult(JsonConvert.SerializeObject(account, Formatting.Indented));
                }
                catch (Exception e)
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
<<<<<<< HEAD
            string acctName  = System.Environment.GetEnvironmentVariable("AcctName", EnvironmentVariableTarget.Process);
            string acctKey   = System.Environment.GetEnvironmentVariable("AcctKey", EnvironmentVariableTarget.Process);
=======
            string acctName = System.Environment.GetEnvironmentVariable("AcctName", EnvironmentVariableTarget.Process);
            string acctKey = System.Environment.GetEnvironmentVariable("AcctKey", EnvironmentVariableTarget.Process);
>>>>>>> develop
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

    public class Account
    {

<<<<<<< HEAD
        public string build = "06/17/2019 07.41.48.431";
=======
        public string build = "06/18/2019 08.00.45.569";
        //public string build = "06/17/2019 15.37.21.769";
        //public string build = "06/17/2019 14.43.40.351";
        //public string build = "06/17/2019 08.388.48.821";
>>>>>>> develop
        private static DateTime dateTime = DateTime.Now;
        public string mode = "";
        public string senderId = "";
        public string method = "";
        public string docType = "";
        public string ext = "";
        public string objName = "";
        public int objLen = 0;
        public string hostName = System.Environment.MachineName.ToLower();
<<<<<<< HEAD
        public string userName = System.Environment.UserName.Replace("Placeholder","az");
=======
        public string userName = System.Environment.UserName;
>>>>>>> develop
        public string function = "BccAzureFunctions";
        public string timestamp = dateTime.ToString("MM/dd/yyyy HH.mm.ss.fff");
        public string fatalException = "";

        public string getObjName()
        {
            objName = getAzurename();
            return objName;
        }

        private string getAzurename()
        {
            string azureName = System.Environment.GetEnvironmentVariable("AzureName", EnvironmentVariableTarget.Process);
            azureName = azureName.Replace(@"${hostname}", hostName);
            azureName = azureName.Replace(@"${function}", function);
            azureName = azureName.Replace(@"${username}", userName);
            azureName = azureName.Replace(@"${date}", dateTime.ToString("yyyyMMdd"));
            azureName = azureName.Replace(@"${time}", dateTime.ToString("HHmmss"));
            azureName = azureName.Replace(@"${timestamp}", dateTime.ToString("yyyyMMddHHmmssfff"));
            azureName = azureName.Replace(@"${length}", objLen.ToString());
            azureName = azureName.Replace(@"${mode}", mode);
            azureName = azureName.Replace(@"${senderid}", senderId);
            azureName = azureName.Replace(@"${method}", method);
            azureName = azureName.Replace(@"${doctype}", docType);
            azureName = azureName.Replace(@"${ext}", ext);

            azureName = azureName.Replace(@"${", "");
            azureName = azureName.Replace(@"}", "");
            azureName = azureName.Replace(@" ", "");
            azureName = azureName.Replace("..", "."); // last edit

            return azureName;

        }
    }
}
