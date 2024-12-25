namespace SharpSite.Abstractions;

public interface IStartupManager
{
	Task LoadAtStartup();
}
