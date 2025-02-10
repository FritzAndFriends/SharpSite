using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using SharpSite.Abstractions;

namespace SharpSite.Data.Postgres;

public class PgPageRepository : IPageRepository
{
	private readonly PgContext Context;
	private readonly IMemoryCache Cache;

	public PgPageRepository(IServiceProvider serviceProvider)
	{
		Context = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<PgContext>();
		Cache = serviceProvider.GetRequiredService<IMemoryCache>();
	}

	public async Task<Page> AddPage(Page page)
	{
		
		// Add the page to the database
		await Context.Pages.AddAsync((PgPage)page);
		await Context.SaveChangesAsync();

		Cache.Remove("Pages");

		return page;

	}

	public async Task DeletePage(int id)
	{
		// delete the page identified with a given id
		var page = Context.Pages.Find(id);

		if (page == null)
		{
			throw new Exception("Page not found");
		}

		Context.Pages.Remove(page);
		await Context.SaveChangesAsync();

		Cache.Remove("Pages");

	}

	public async Task<Page?> GetPage(string slug)
	{

		// check if the page is in the cache
		if (Cache.TryGetValue("Pages", out IEnumerable<Page>? pages))
		{
			return pages!.FirstOrDefault(p => p.Slug == slug);
		}

		// get the page with a given slug
		var page = await Context.Pages
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Slug == slug);

		// check for a page with the given slug
		if (page == null)
		{
			return null;
		}

		return (Page?)page;

	}

	public async Task<Page?> GetPage(int id)
	{
		
		// get the page with a given id
		var page = await Context.Pages
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Id == id);

		return page is not null ? (Page?)page : null;

	}

	public async Task<IEnumerable<Page>> GetPages()
	{

		// check if the pages are in the cache
		return await Cache.GetOrCreateAsync("Pages", async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
			var pages = await Context.Pages
				.AsNoTracking()
				.Select(p => (Page)p)
				.ToListAsync();
	
			return pages ?? Enumerable.Empty<Page>();
		}) ?? Enumerable.Empty<Page>();

	}

	public async Task<IEnumerable<Page>> GetPages(Expression<Func<Page, bool>> where)
	{

		// get all pages from the database that satisfy the given condition
		var pages = await GetPages();
		return pages
			.Where(p => where.Compile().Invoke((Page)p))
			.Select(p => (Page)p)
			.ToList();

	}

	public async Task UpdatePage(Page page)
	{
		
		// update the existing page in the database
		page.LastUpdate = DateTimeOffset.Now;
		Context.Pages.Update((PgPage)page);
		await Context.SaveChangesAsync();

		Cache.Remove("Pages");
		
	}
}