using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Entities
{
    public class PasswordResetToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiryTime { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; }

        
        // Navigation Property

        public User User { get; set; } = null!;
    }
}
