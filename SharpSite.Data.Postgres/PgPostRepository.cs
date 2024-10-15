﻿using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SharpSite.Abstractions;

namespace SharpSite.Data.Postgres;

public class PgPostRepository(PgContext Context) : IPostRepository
{
	public async Task<Post> AddPost(Post post)
	{
		// add a post to the database
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

	public async Task<Post?> GetPost(string slug)
	{
		// get a post from the database based on the slug submitted
		return await Context.Posts
			.Where(p => p.Slug == slug)
			.Select(p => (Post)p)
			.FirstOrDefaultAsync();
		
	}

	public async Task<IEnumerable<Post>> GetPosts()
	{
		// get all posts from the database
		var posts = await Context.Posts.ToArrayAsync();
		return posts.Select(p => (Post)p);
	}

	public async Task<IEnumerable<Post>> GetPosts(Expression<Func<Post, bool>> where)
	{
		// get all posts from the database based on the where clause
		return await Context.Posts
			.Where(p => where.Compile().Invoke((Post)p))
			.Select(p => (Post)p)
			.ToArrayAsync();

	}

	public async Task<Post> UpdatePost(Post post)
	{
		// update a post in the database
		Context.Posts.Update((PgPost)post);
		await Context.SaveChangesAsync();

		return post;

	}
}
