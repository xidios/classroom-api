using classroom_api.Enums;

namespace classroom_api.Models
{
    public class InvitationModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AccountId { get; set; }
        public Guid CourseId { get; set; }
        public CourseModel Course { get; set; }
        public Guid CourseUserId { get; set; }
        public CourseUserModel CourseUser { get; set; }
        public string Email { get; set; }
        public CourseRoleEnum Role { get; set; }
        public string Status { get; set; } = "OK";
        public DateTime Created { get; set; }
        public string GoogleInvitationId { get; set; }
    }
}
