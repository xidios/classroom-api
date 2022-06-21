using Newtonsoft.Json;

namespace classroom_api.FromBodyModels
{
    public class InvitePersonModel
    {
        public Guid CourseId { get; set; }
        public Guid AccountId { get; set; }       
    }

    public class InviteTeachersModel
    {
        public Guid CourseId { get; set; }
        public List<Guid> AccountIds { get; set; }
    }

    public class InviteGroupModel
    {
        public Guid CourseId { get; set; }
        public List<string> AccountIdList { get; set; }

    }
    public class InviteFormBodyDeleteModel
    {
        public List<string> InviteId { get; set; }
    }
}
