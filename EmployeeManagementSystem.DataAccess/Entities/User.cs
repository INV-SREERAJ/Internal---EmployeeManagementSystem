using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId {  get; set; }
        public int? ManagerId { get; set; }

        //used to check if the refresh token is still active
        public int TokenVersion { get; set; }

        //used to make the user reset the password in the initial login
        public bool MustChangePassword { get; set; }
        public bool IsActive { get; set; }

        //soft delete
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public Role Role { get; set; } = null;
        public User? Manager { get; set; }
        public ICollection<User> Employees { get; set; } = new List<User>();
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; }
        = new List<PasswordResetToken>();
    }
}
