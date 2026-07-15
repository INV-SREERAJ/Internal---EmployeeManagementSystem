using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        //1 to many
        public ICollection<User> Users = new List<User>();
    }
}
