using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharpSite.Abstractions;
using System.Globalization;
using System.Linq.Expressions;

namespace SharpSite.Data.Postgres;

public class PgPostRepository : IPostRepository
{

	public PgPostRepository(IServiceProvider serviceProvider)
	{
		Context = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<PgContext>();
	}

	private readonly PgContext Context;

	public async Task<Post> AddPost(Post post)
	{
		// add a post to the database
		post.PublishedDate = DateTimeOffset.Now;
		post.LastUpdate = DateTimeOffset.Now;
		await Context.Posts.AddAsync((PgPost)post);
		await Context.SaveChangesAsync();

		return post;
	}

	public async Task DeletePost(string slug)
	{
		// delete a post from the database based on the slug submitted
		var post = await Context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
		if (post != null)
		{
			Context.Posts.Remove(post);
			await Context.SaveChangesAsync();
		}
	}

	public async Task<Post?> GetPost(string dateString, string slug)
	{

		if (string.IsNullOrEmpty(dateString) || string.IsNullOrEmpty(slug))
		{
			return null;
		}

		var theDate = DateTimeOffset.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);

		// get a post from the database based on the slug submitted
		var thePosts = await Context.Posts
			.AsNoTracking()
			.Where(p => p.Slug == slug)
			.Select(p => (Post)p)
			.ToArrayAsync();

		return thePosts.FirstOrDefault(p =>
			p.PublishedDate.UtcDateTime.Date == theDate.UtcDateTime.Date);

	}

	public async Task<IEnumerable<Post>> GetPosts()
	{
		// get all posts from the database
		var posts = await Context.Posts.AsNoTracking().ToArrayAsync();
		return posts.Select(p => (Post)p);
	}

	public async Task<IEnumerable<Post>> GetPosts(Expression<Func<Post, bool>> where)
	{
		// get all posts from the database based on the where clause
		return await Context.Posts
			.AsNoTracking()
			.Where(p => where.Compile().Invoke((Post)p))
			.Select(p => (Post)p)
			.ToArrayAsync();

	}

	public async Task<Post> UpdatePost(Post post)
	{
		// update a post in the database
		post.LastUpdate = DateTimeOffset.Now;
		Context.Posts.Update((PgPost)post);
		await Context.SaveChangesAsync();

		return post;

	}
}
