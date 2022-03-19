namespace classroom_api.Models
{
    public class ClassroomCreateFromBodyModel
    {
        public string? Name { get; set; }
        public string? Section { get; set; }
        public string? DescriptionHeading { get; set; }
        public string? Description { get; set; }
        public string? Room { get; set; }
        //public string OwnerId { get; set; } = "me";
        //public string? CourseState { get; set; }
    }
}
