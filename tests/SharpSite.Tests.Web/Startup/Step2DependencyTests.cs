using System.Reflection;
using Microsoft.AspNetCore.Components;
using SharpSite.Abstractions.FileStorage;
using Xunit;

namespace SharpSite.Tests.Web.Startup;

public class Step2DependencyTests
{
	[Fact]
	public void Step2_ShouldNotInjectFileStorageDirectly()
	{
		// Arrange
		var componentType = typeof(SharpSite.Web.Components.Startup.Step2);

		// Act
		var injectedFileStorageProperties = componentType
			.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			.Where(property =>
				property.PropertyType == typeof(IHandleFileStorage) &&
				property.GetCustomAttribute<InjectAttribute>() is not null);

		// Assert
		Assert.Empty(injectedFileStorageProperties);
	}

	[Fact]
	public void Step2_ShouldInjectPluginManagerForOptionalFileStorageLookup()
	{
		// Arrange
		var componentType = typeof(SharpSite.Web.Components.Startup.Step2);

		// Act
		var pluginManagerProperty = componentType
			.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			.SingleOrDefault(property =>
				property.PropertyType == typeof(SharpSite.Web.PluginManager) &&
				property.GetCustomAttribute<InjectAttribute>() is not null);

		// Assert
		Assert.NotNull(pluginManagerProperty);
	}
}
