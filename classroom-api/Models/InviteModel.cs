using Newtonsoft.Json;

namespace classroom_api.Models
{
    public class InviteModel
    {
        public string CourseId { get; set; }
        //public string EnrollmentCode { get; set; }
        [JsonProperty(PropertyName = "AccountId")]
        public List<string> AccountIdList { get; set; }
        
    }
}
