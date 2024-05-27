using FileStoragePOC.Enums;
using FileStoragePOC.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

namespace FileStoragePOC
{
    public class Functions
    {
        [Function(nameof(Orchestrator))]
        public async Task Orchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context, ILogger log)
        {

            //ExportFile exportFile = new ExportFile();
            // byte[] content = Encoding.UTF8.GetBytes("Hello World);

            //exportFile.FileContent = content;
            //exportFile.FileName = "Transaction_16052024.txt";
            //exportFile.FileType = ExportFileType.OffShore;

            //await context.CallActivityAsync("StoreFileToBlobStorage", exportFile);

            //==============================================================================================================================


            ExportFile exportFile = new ExportFile();
            string[] file = { "Hello World~", "Nitesh Jha~","abc!~","~abcc","~jkgjgjkkk~~~" };


            byte[] content = ConvertToByteArray(file);

            exportFile.FileContent = content;
            exportFile.FileName = "Transaction_2024.txt";
            exportFile.FileType = ExportFileType.OnShore;

            await context.CallActivityAsync("StoreFileToBlobStorage", exportFile);


            EncryptFileServices _encryptFileServices = new();

            var encryptionKey = _encryptFileServices.GetPublicTestKey();
            var encryptedFileContent = await _encryptFileServices.EncryptFile(content, encryptionKey);

            exportFile.FileContent = encryptedFileContent;
            exportFile.FileName = "Transaction_2024.txt.pgp";

            //await context.CallActivityAsync("StoreFileToBlobStorage", exportFile);


             var decryptedContent = await _encryptFileServices.DecryptFile(encryptedFileContent);


            exportFile.FileContent = decryptedContent;
            exportFile.FileName = "Transaction_2024(decrypt).txt";

            //await context.CallActivityAsync("StoreFileToBlobStorage", exportFile);

        }

        [Function("StarterFunction")]
        public async Task<HttpResponseData> StarterFunction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(Orchestrator));

            return client.CreateCheckStatusResponse(req, instanceId);
        }

        private byte[] ConvertToByteArray(string?[] rowData)
        {
            byte[] fileContent;//= rowData.SelectMany(data => Encoding.GetEncoding(1252).GetBytes(data + "\n")).ToArray(); 
            using (MemoryStream stream = new MemoryStream())
            {
                // Iterate over each string
                foreach (string data in rowData)
                {
                    // Convert the string to bytes and write it to the memory stream
                    byte[] line = Encoding.GetEncoding(1252).GetBytes(data + Environment.NewLine);
                    stream.Write(line, 0, line.Length);
                }

                // Get the byte array representing the content of the memory stream
                fileContent = stream.ToArray();
            }
            return fileContent;
        }


    }
}
