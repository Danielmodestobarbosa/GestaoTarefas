
namespace Tarefas.Application.Common.Pagination
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = [];

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        // Construtor utilizado pelo serializador JSON
        public PagedResult()
        {
        }

        // Construtor utilizado pela aplicação
        public PagedResult(
            List<T> items,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;

            TotalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);
        }
    }
}

