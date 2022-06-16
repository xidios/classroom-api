namespace classroom_api.Models
{
    public class EmailModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public UserModel User { get; set; }
        public bool IsMain { get; set; }
    }
}
