namespace classroom_api.Models.TSU
{
    public class TSUStudent
    {
        public Guid AccountId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
    }
}
