namespace SharpSite.Abstractions;

public static class Constants
{

	public static class Roles
	{
		public const string AdminUsers = "Admin";
		public const string Admin = "Admin";
		public const string Editor = "Editor";
		public const string EditorUsers = "Admin,Editor";
		public const string User = "User";
		public const string AllUsers = "Admin,Editor,User";

		public static string[] AllRoles = [Admin, Editor, User];

	}

}