﻿namespace classroom_api.Models
{
    public class InvitationModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CourseId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Status { get; set; } = "OK";
        public string GoogleInvitationId { get; set; }
    }
}
