using StockExchangeSystem.API.Utilities.Interfaces;

namespace StockExchangeSystem.API.Utilities
{
    public class PaginationHelper : IPaginationHelper
    {
        public IQueryable<T> Paginate<T>(IQueryable<T> query, int page_size, int page_number)
        {
            if (page_size <= 0 || page_number <= 0)
            {
                throw new ArgumentException("Invalid paging parameters. Page size and page number should be greater than 0.");
            }

            return query.Skip((page_number - 1) * page_size).Take(page_size);
        }
    }
}
