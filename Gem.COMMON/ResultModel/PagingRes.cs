namespace Gem.COMMON.ResultModel
{
    public class PagingRes<T> : ResModel
    {
        public ICollection<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public int TotalRecords { get; set; }
    }
}
