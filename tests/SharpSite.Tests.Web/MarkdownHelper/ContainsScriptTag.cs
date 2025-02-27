using Xunit;

namespace SharpSite.Tests.Web.MarkdownHelper;

public class ContainsScriptTag
{

    [Theory]
    [InlineData("<script src='test.js'></script>")]
    [InlineData("<script src='https://cdn.example.com/script.js'></script>")]
    [InlineData("<SCRIPT>alert('test')</SCRIPT>")]
    [InlineData("<script>\nvar x = 1;\n</script>")]
    [InlineData("Text before <script>code</script> text after")]
    [InlineData("<div><script>test</script></div>")]
    [InlineData("<script type=\"text/javascript\">code</script>")]
    [InlineData("<SCRIPT language='JavaScript'>code</SCRIPT>")]
    [InlineData("Text\n<script>code</script>\nMore text")]
    public void WithValidScriptTagsReturnsTrue(string markdown)
    {
        Assert.True(SharpSite.Web.MarkdownHelper.ContainsScriptTag(markdown));
    }

	[Theory]
	[InlineData("```html\n<script>alert('test')</script>\n```")]
	[InlineData("`const script = '<script></script>'`")]
	[InlineData("<scrip>not a script tag</scrip>")]
	[InlineData("<script>incomplete tag")]
	[InlineData("incomplete tag </script>")]
	[InlineData("```js\nlet script = document.createElement('script');\n```")]
	[InlineData("`<script>`")]
	[InlineData("```\n<script>test</script>\n<script>test2</script>\n```")]
	[InlineData("<script-custom>not a real script tag</script-custom>")]
	public void WithInvalidOrEscapedScriptTagsReturnsFalse(string markdown)
	{
		Assert.False(SharpSite.Web.MarkdownHelper.ContainsScriptTag(markdown));
	}

	[Theory]
	[InlineData("# Just Markdown")]
	[InlineData("Just text")]
	[InlineData("## Script Documentation\nThis is about scripts")]
	[InlineData("*italic* **bold** [link](https://test.com)")]
	public void WithJustValidMarkdownReturnsFalse(string markdown)
	{
		Assert.False(SharpSite.Web.MarkdownHelper.ContainsScriptTag(markdown));
	}

}
