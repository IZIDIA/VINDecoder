using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
namespace VIN_decoder.Classes.VIN {
	class VINParser : IParser<string[]> {
		static string NormalizeWhiteSpace(string input, char normalizeTo = ' ') {
			if (string.IsNullOrEmpty(input)) {
				return string.Empty;
			}
			int current = 0;
			char[] output = new char[input.Length];
			bool skipped = false;
			foreach (char c in input.ToCharArray()) {
				if (char.IsWhiteSpace(c)) {
					if (!skipped) {
						if (current > 0)
							output[current++] = normalizeTo;

						skipped = true;
					}
				}
				else {
					skipped = false;
					output[current++] = c;
				}
			}
			return new string(output, 0, skipped ? current - 1 : current);
		}
		public string[] Parse(IHtmlDocument document) {
			var list = new List<string>();
			var name = document.QuerySelectorAll("h1").Where(item => item.ClassName != null && item.ClassName.Contains("fw-light mb-0"));
			var items = document.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("w-100 border-1 border-bottom-dashed"));
			foreach (var item in name) {
				list.Add(NormalizeWhiteSpace(item.TextContent));
			}
			foreach (var item in items) {
				list.Add(NormalizeWhiteSpace(item.TextContent));
			}
			return list.ToArray();
		}
	}
}
