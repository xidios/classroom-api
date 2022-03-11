namespace classroom_api.Models
{
    public class StatisticModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int TeacherInvitationCount { get; set; }
        public int StudentsInvitationCount { get; set; }
        public int ClassesCreatedCount { get; set; }
    }
}
