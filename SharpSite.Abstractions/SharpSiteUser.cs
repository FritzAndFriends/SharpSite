namespace SharpSite.Abstractions;

public class SharpSiteUser
{
	public SharpSiteUser(string id, string? userName, string? email)
	{
		Id = id;
		UserName = userName;
		Email = email;
	}

	public string Id { get; }
	public string? UserName { get; }
	public string? Email { get; }
}
