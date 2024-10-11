namespace SharpSite.Abstractions;

public interface IPostRepository
{

	Post GetPost(string slug);

	IEnumerable<Post> GetPosts();

	IQueryable<Post> GetPosts(Func<Post, bool> where);

	Post AddPost(Post post);

	Post UpdatePost(Post post);

	void DeletePost(string slug);

}
