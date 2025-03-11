using System.Text.RegularExpressions;

namespace SharpSite.Web;

public static partial class MarkdownHelper
{
	/// <summary>
	/// Checks if the markdown contains script tags outside of inline code or code blocks.
	/// </summary>
	/// <param name="markdown">The markdown to check.</param>
	/// <returns></returns>
	public static bool ContainsScriptTag(string markdown)
	{

		markdown = CodeBlockRegex().Replace(markdown, string.Empty);
		markdown = InlineCodeRegex().Replace(markdown, string.Empty);

		bool containsOpeningScriptTag = ScriptTagOpeningRegex().IsMatch(markdown);
		bool containsClosingScriptTag = markdown.Contains("</script>", StringComparison.OrdinalIgnoreCase);
		return containsOpeningScriptTag && containsClosingScriptTag;

	}

	[GeneratedRegex(@"<script\b[^>]*>", RegexOptions.IgnoreCase)]
	private static partial Regex ScriptTagOpeningRegex();

	[GeneratedRegex(@"```[\s\S]*?```")]
	private static partial Regex CodeBlockRegex();

	[GeneratedRegex(@"`[^`]*`")]
	private static partial Regex InlineCodeRegex();
}

