using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.Business.GlobalExceptionHandler
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base (message) { 
        
        }
    }
}
