namespace StockExchangeSystem.API.Utilities.Interfaces
{
    public interface IPaginationHelper
    {
        IQueryable<T> Paginate<T>(IQueryable<T> query, int page_size, int page_number);
    }
}
