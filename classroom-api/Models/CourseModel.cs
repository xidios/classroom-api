namespace classroom_api.Models
{
    public class CourseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionHeading { get; set; }
        public string Section { get; set; }
        public string CourseState { get; set; }
        public string Status { get; set; } = "OK";
        public List<StudentModel> Students { get; set; }
    }
}
