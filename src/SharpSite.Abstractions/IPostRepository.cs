using System.Linq.Expressions;

namespace SharpSite.Abstractions;

public interface IPostRepository
{

	Task<Post?> GetPost(string dateString, string slug);

	Task<IEnumerable<Post>> GetPosts();

	Task<IEnumerable<Post>> GetPosts(Expression<Func<Post, bool>> where);

	Task<Post> AddPost(Post post);

	Task<Post> UpdatePost(Post post);

	Task DeletePost(string slug);

}
