using SharpSite.Abstractions.Theme;

namespace Sample.FirstThemePlugin;

public class MyTheme : IHasStylesheets
{

	public string[] Stylesheets => [
		"theme.css"
	];

}
