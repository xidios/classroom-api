using Newtonsoft.Json;

namespace classroom_api.FromBodyModels
{
    public class InvitePersonModel
    {
        public string CourseId { get; set; }
        [JsonProperty(PropertyName = "AccountId")]
        public string AccountId { get; set; }
        
    }

    public class InviteGroupModel
    {
        public string CourseId { get; set; }
        public List<string> AccountIdList { get; set; }

    }
    public class InviteFormBodyDeleteModel
    {
        public List<string> InviteId { get; set; }
    }
}
