namespace LocadoraDeAutomoveis.Domain.Shared;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public PagedResult() { }

    public PagedResult(List<T> items, int totalCount, int currentPage, int pageSize) : this()
    {
        this.Items = items;
        this.TotalCount = totalCount;
        this.CurrentPage = currentPage;
        this.PageSize = pageSize;

        this.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}