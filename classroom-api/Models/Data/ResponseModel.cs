namespace classroom_api.Models
{
    public class ResponseModel
    {
        public string Status { get; set; }
        public string? Error { get; set; }
        public ResponseModel(string status,string? error = null)
        {
            this.Status = status;
            this.Error = error;
        }
    }
}
