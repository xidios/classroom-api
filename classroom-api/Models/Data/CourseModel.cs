using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace classroom_api.Models
{
    public class CourseModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? DescriptionHeading { get; set; }
        public string? Section { get; set; }
        public string CourseState { get; set; }
        public string Status { get; set; } = "OK";
        [Required]
        public string GoogleId { get; set; }
        public ICollection<InvitationModel> InvitationsOnCourse { get; set; } = new List<InvitationModel>();
        public Guid? SubdivisionId { get; set; }
        public SubdivisionModel? Subdivision { get; set; }
    }
}
