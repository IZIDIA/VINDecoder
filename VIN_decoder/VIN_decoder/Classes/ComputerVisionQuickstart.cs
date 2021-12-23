using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace VIN_decoder.Classes {
	public class ComputerVisionQuickstart {
		static public string subscriptionKey = "d9d070d747824d4ba98a2f29c1f74c93";
		static public string endpoint = "https://vindecoder.cognitiveservices.azure.com/";
		static public string textResultsInString = "null";
		static public ComputerVisionClient client;
		public static Task<string> StartService(string localFile) {
			return Task.Run(async () => {
				var textHeaders = await client.ReadInStreamAsync(File.OpenRead(localFile));
				string operationLocation = textHeaders.OperationLocation;
				const int numberOfCharsInOperationId = 36;
				string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
				ReadOperationResult results;
				do {
					results = await client.GetReadResultAsync(Guid.Parse(operationId));
				}
				while (results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted);
				var textUrlFileResults = results.AnalyzeResult.ReadResults;
				textResultsInString = "";
				foreach (ReadResult page in textUrlFileResults) {
					foreach (Line line in page.Lines) {
						textResultsInString += line.Text + "\n";
					}
				}
				return textResultsInString;
			});
		}
		public static ComputerVisionClient Authenticate(string endpoint, string key) {
			ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key)) {
				Endpoint = endpoint
			};
			return client;
		}
	}
}
