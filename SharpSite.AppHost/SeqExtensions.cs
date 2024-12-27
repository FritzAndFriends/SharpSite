public static class SeqExtensions
{
	/// <summary>
	/// A collection of version information used by the containers in this app
	/// </summary>
	public static class SEQ
	{
		public const int API_PORT = 5341;
		public const int HTTP_PORT = 8080;
	}

	/// <summary>
	/// Adds a Seq container to the distributed application.
	/// </summary>
	/// <param name="builder">The distributed application builder.</param>
	/// <returns>A resource builder for the Seq container.</returns>
	public static IResourceBuilder<SeqResource> AddSeqServices(this IDistributedApplicationBuilder builder)
	{
		// Add Seq container
		var seq = builder.AddSeq("seq").WithEndpoint("http", endpoint =>
			{
				endpoint.Port = SEQ.HTTP_PORT; // Host port
				endpoint.TargetPort = 80; // Container port
				endpoint.UriScheme = "http"; // URI scheme
			}).WithEndpoint("api", endpoint =>
			{
				endpoint.Port = SEQ.API_PORT; // Host port
				endpoint.TargetPort = 5341; // Container port
				endpoint.UriScheme = "http"; // URI scheme
			})
			.WithExternalHttpEndpoints(); // Expose HTTP endpoints
		// Wrap the container resource in a SeqResource
		return seq;
	}
}