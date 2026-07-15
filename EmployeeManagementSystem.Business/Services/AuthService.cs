using EmployeeManagementSystem.Business.DTOs.Auth;
using EmployeeManagementSystem.Business.Interfaces;
using EmployeeManagementSystem.DataAccess.Interfaces;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return false;
            }

            return true;
        }
    }
}