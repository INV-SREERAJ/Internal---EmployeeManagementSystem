using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.GlobalExceptionHandler
{
    public class UnAuthorizedException : Exception
    {
        public UnAuthorizedException(string message) : base(message) { }
    }
}
