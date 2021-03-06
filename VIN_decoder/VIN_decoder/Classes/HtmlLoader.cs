using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace VIN_decoder.Classes {
	class HtmlLoader {
		readonly HttpClient client;
		readonly string url;
		public HtmlLoader(IParserSettings settings) {
			client = new HttpClient();
			url = $"{settings.BaseUrl}/{settings.Prefix}/";
		}
		public async Task<string> GetSourceByPageId(string id) {
			var currentUrl = url.Replace("{CurrentId}", id);
			var response = await client.GetAsync(currentUrl);
			string source = null;
			if (response != null && response.StatusCode == HttpStatusCode.OK) {
				source = await response.Content.ReadAsStringAsync();
			}
			return source;
		}
	}
}
