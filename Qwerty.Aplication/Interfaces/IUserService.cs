using Qwerty.BLL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IUserService
    {
        Task CreateAsync(UserDTO userDto, string password);
        Task<UserDTO> LoginAsync(string UserName, string Password);
        Task ChangeProfileInformation(UserDTO userDto);
        Task<UserDTO> FindAsync(string userName);
        Task UploadImage(string imageUrl, string userName);
        Task<UserDTO> FindByIdAsync(string userId);
        Task<IList<string>> GetRolesByUserId(string id);
        Task DeleteUser(string userId);
        Task RestoreAccount(string userId);
        Task<IEnumerable<UserDTO>> GetUsersWithoutFriends(UserSearchParametersDto searchParameters);
    }
}
