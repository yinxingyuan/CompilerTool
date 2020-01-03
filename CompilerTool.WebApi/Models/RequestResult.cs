namespace CompilerTool.WebApi.Models
{
    public class RequestResult
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}