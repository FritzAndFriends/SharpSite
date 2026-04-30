using System.Reflection;
using Xunit.Sdk;

namespace SharpSite.E2E;

public class WithTestNameAttribute : BeforeAfterTestAttribute
{
	public static string CurrentTestName = string.Empty;
	public static string CurrentClassName = string.Empty;

	public override void Before(MethodInfo methodInfo)
	{
		CurrentTestName = methodInfo.Name;
		CurrentClassName = methodInfo.DeclaringType!.Name;
	}

	public override void After(MethodInfo methodInfo)
	{
	}
}
