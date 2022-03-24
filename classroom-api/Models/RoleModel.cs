using System.Text.Json.Serialization;

namespace classroom_api.Models
{
    public class RoleModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public List<PermissionModel> Permissions { get; set; }
        [JsonIgnore]
        public List<UserModel> Users { get; set; } = new List<UserModel>();
    }
    public class PermissionModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Action { get; set; }
        [JsonIgnore]
        public List<RoleModel> Roles { get; set; } = new List<RoleModel>();
    }
}
