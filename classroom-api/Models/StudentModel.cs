namespace classroom_api.Models
{
    public class StudentModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AccountId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<CourseModel> Courses { get; set; }
    }
}
