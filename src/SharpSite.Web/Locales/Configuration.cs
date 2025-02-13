namespace SharpSite.Web.Locales;
public static class Configuration
{

	public readonly static string[] SupportedCultures = [
		"bg",
		"en",
		"es",
		"fi",
		"fr",
		"it",
		"nl",
		"pt",
		"sv",
		"sw",
		"de",
		"ca",
	];

	/// <summary>
	/// add the custom localization features for the application framework
	/// </summary>
	/// <param name="builder"></param>
	public static void ConfigureRequestLocalization(this WebApplicationBuilder builder)
	{

		var appState = builder.Services.BuildServiceProvider().GetRequiredService<ApplicationState>();
		var cultures = appState.Localization?.SupportedCultures ?? SupportedCultures;
		var defaultCulture = appState.Localization?.DefaultCulture ?? "en";
		builder.Services.Configure<RequestLocalizationOptions>(options =>
		{
			options.SetDefaultCulture(defaultCulture)
									.AddSupportedCultures(cultures)
									.AddSupportedUICultures(cultures);
		});

		builder.Services.AddLocalization(options =>
		{
			options.ResourcesPath = "Locales";
		});

	}
}
