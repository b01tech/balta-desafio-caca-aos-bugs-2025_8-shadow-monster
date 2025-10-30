namespace BugStore.Application.Extensions
{
    public static class PaginationExtensions
    {
        public static int CalculateTotalPages(this long totalCount, int pageSize)
        {
            return (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}
