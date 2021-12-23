using AngleSharp.Html.Dom;

namespace VIN_decoder.Classes {
	interface IParser<T> where T : class {
		T Parse(IHtmlDocument document);
	}
}
