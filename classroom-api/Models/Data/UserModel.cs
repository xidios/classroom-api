﻿using System.Text.Json.Serialization;

namespace classroom_api.Models
{
    public class UserModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string? Token { get; set; }
        public List<SubdivisionModel> Subdivisions { get; set; } = new List<SubdivisionModel>();
        public List<EmailModel> Emails { get; set; } = new List<EmailModel>();
        public List<RoleModel> Roles { get; set; } = new List<RoleModel>();
    }
}