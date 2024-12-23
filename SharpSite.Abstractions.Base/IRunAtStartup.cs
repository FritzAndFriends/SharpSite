namespace SharpSite.Abstractions.Base;

/// <summary>
/// Interface for services that need to run at startup of the web application.
/// </summary>
public interface IRunAtStartup
{
	Task RunAtStartup(IServiceProvider services);
}

public interface IHasEndpoints
{
	void MapEndpoints(IServiceProvider services);
}
