public static class RouteValues
{
	public const string AdminPostList = "/admin/posts";
}

public record struct RouteValue(string Value, Func<string, string>? Formatter)
{

	public RouteValue(string value) : this(value, null) { }

	public override string ToString() => Value;

	public static implicit operator string(RouteValue value) => value.ToString();

}