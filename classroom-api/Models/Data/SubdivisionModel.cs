namespace classroom_api.Models
{
    public class SubdivisionModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public List<CourseModel> Courses { get; set; } = new List<CourseModel>();
        public List<UserModel> Moderators { get; set; } = new List<UserModel>();
    }
}
