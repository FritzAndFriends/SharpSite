namespace SharpSite.Web.Locales;
public static class Configuration
{

	public readonly static string[] SupportedCultures = { 
		"en-US", 
		"fr-FR",
		"nl-NL",
		"bg-BG"
	};

	/// <summary>
	/// add the custom localization features for the application framework
	/// </summary>
	/// <param name="builder"></param>
	public static void ConfigureRequestLocalization(this WebApplicationBuilder builder)
	{

		builder.Services.Configure<RequestLocalizationOptions>(options =>
		{
			
			options.SetDefaultCulture(SupportedCultures[0])
									.AddSupportedCultures(SupportedCultures)
									.AddSupportedUICultures(SupportedCultures);
		});

		builder.Services.AddLocalization(options =>
		{
			options.ResourcesPath = "Locales";
		});

	}
}