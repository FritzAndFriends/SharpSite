using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

public static class RunE2ETestsCommand
{
	public const string Name = "run-e2e-tests";

	public static IResourceBuilder<ProjectResource> WithRunE2eTestsCommand(
			this IResourceBuilder<ProjectResource> builder)
	{
		builder.WithCommand(
				name: Name,
				displayName: "Run end to end tests",
				executeCommand: context => RunTests(),
				updateState: OnUpdateResourceState,
				iconName: "BookGlobe",
				iconVariant: IconVariant.Filled);

		return builder;
	}


	private static async Task<ExecuteCommandResult> RunTests()
	{
		var processStartInfo = new ProcessStartInfo
		{
			FileName = "dotnet",
			Arguments = "test ../../e2e/SharpSite.E2E",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		var process = new Process { StartInfo = processStartInfo };
		process.Start();

		var output = await process.StandardOutput.ReadToEndAsync();
		var error = await process.StandardError.ReadToEndAsync();

		process.WaitForExit();
		Console.WriteLine("E2E Tests Output: " + output);

		if (process.ExitCode == 0)
		{
			return new ExecuteCommandResult() { Success = true };
		}
		else
		{
			return new ExecuteCommandResult() { Success = false, ErrorMessage = error };
		}
	}

	private static ResourceCommandState OnUpdateResourceState(
		UpdateCommandStateContext context)
	{
		var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();

		//if (logger.IsEnabled(LogLevel.Information))
		//{
		//	logger.LogInformation(
		//			"Updating resource state: {ResourceSnapshot}",
		//			context.ResourceSnapshot);
		//}

		return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
				? ResourceCommandState.Enabled
				: ResourceCommandState.Disabled;
	}

}
