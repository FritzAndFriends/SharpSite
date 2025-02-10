using SUT = SharpSite.Web.ApplicationState;


namespace SharpSite.Tests.Web.ApplicationState;

public abstract class BaseFixture
{
	protected SUT ApplicationState { get; set; }
	protected BaseFixture()
	{
		ApplicationState = new SUT();
	}
}
