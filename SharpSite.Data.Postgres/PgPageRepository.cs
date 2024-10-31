using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SharpSite.Abstractions;

namespace SharpSite.Data.Postgres;

public class PgPageRepository(PgContext Context) : IPageRepository
{
	public async Task<Page> AddPage(Page page)
	{
		
		// Add the page to the database
		await Context.Pages.AddAsync((PgPage)page);
		await Context.SaveChangesAsync();

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

	}

	public async Task<Page?> GetPage(string slug)
	{
		
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

	public async Task<IEnumerable<Page>> GetPages()
	{
		// get all pages from the database
		return (await Context.Pages.AsNoTracking().ToListAsync())
			.Select(p => (Page)p)
			.ToList();

	}

	public async Task<IEnumerable<Page>> GetPages(Expression<Func<Page, bool>> where)
	{

		// get all pages from the database that satisfy the given condition
		return await Context.Pages
			.AsNoTracking()
			.Where(p => where.Compile().Invoke((Page)p))
			.Select(p => (Page)p)
			.ToListAsync();

	}

}