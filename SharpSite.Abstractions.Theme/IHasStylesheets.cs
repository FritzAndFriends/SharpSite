namespace SharpSite.Abstractions.Theme;

/// <summary>
/// Represents part of a theme that defines the stylesheets to be included
/// </summary>
public interface IHasStylesheets
{

	/// <summary>
	/// a collection of URLs that point to the stylesheets to be included
	/// </summary>
	string[] Stylesheets { get; }
}
