using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.GlobalExceptionHandler
{
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
