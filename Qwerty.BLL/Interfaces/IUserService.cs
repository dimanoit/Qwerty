using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IUserService : IDisposable
    {
        Task<OperationDetails> Create(UserDTO userDto);
        Task<ClaimsIdentity> Authenticate(UserDTO userDto);
        Task SetInitialData(UserDTO adminDto, List<string> roles);
        Task<UserDTO> FindUser(string UserName, string Password);
        Task<OperationDetails> ChangeProfileInformation(UserDTO userDTO);
        Task<OperationDetails> DeleteUser(string UserId);
        Task<IEnumerable<UserDTO>> GetUsersByFullName(string Name, string Surname);
        Task<UserDTO> FindUserByUsername(string UserName);
    }
}
