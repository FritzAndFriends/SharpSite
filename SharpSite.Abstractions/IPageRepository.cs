using System.Linq.Expressions;

namespace SharpSite.Abstractions;

public interface IPageRepository
{
	
	Task<Page> AddPage(Page page);
	Task UpdatePage(Page page);
	Task DeletePage(int id);
	Task<Page?> GetPage(string slug);
	Task<Page?> GetPage(int id);
	Task<IEnumerable<Page>> GetPages();
	Task<IEnumerable<Page>> GetPages(Expression<Func<Page, bool>> where);

}