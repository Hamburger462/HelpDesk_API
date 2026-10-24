namespace HelpDesk.Data.Dto
{
    public record PagedRequest
    {
        /// <summary>Page number, starting at 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Items per page (1 to 100).</summary>
        public int PageSize { get; set; } = 20;
    }

    public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
    {
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
