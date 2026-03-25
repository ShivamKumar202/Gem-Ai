namespace Gem.COMMON.ResultModel
{
    public class ResModel
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
    public class ResModel<T> : ResModel
    {
        public T Data { get; set; }
    }
    public class ResListModel<T> : ResModel
    {
        public ICollection<T> Data { get; set; }
    }
}
