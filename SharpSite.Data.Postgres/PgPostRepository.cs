using SharpSite.Abstractions;

namespace SharpSite.Data.Postgres;

public class PgPostRepository : IPostRepository
{
	public Post AddPost(Post post)
	{
		throw new NotImplementedException();
	}

	public void DeletePost(string slug)
	{
		throw new NotImplementedException();
	}

	public Post GetPost(string slug)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<Post> GetPosts()
	{
		throw new NotImplementedException();
	}

	public IQueryable<Post> GetPosts(Func<Post, bool> where)
	{
		throw new NotImplementedException();
	}

	public Post UpdatePost(Post post)
	{
		throw new NotImplementedException();
	}
}
