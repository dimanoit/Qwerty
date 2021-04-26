using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IUserService
    {
        Task CreateUserAsync(UserDTO userDto);
        Task<UserDTO> FindUserAsync(string UserName, string Password);
        Task ChangeProfileInformation(UserDTO userDTO);
        Task<IEnumerable<UserDTO>> GetUsers(string Name = null, string Surname = null, string Country = null, string City = null);
        UserDTO FindUserByUsername(string UserName);
        Task UploadImage(string ImageUrl, string UserName);
        Task<UserDTO> FindUserByIdAsync(string UserId);
        Task<IList<string>> GetRolesByUserId(string id);
        Task DeleteUser(string userId);
        Task RestoreAccount(string userId);
        Task<IEnumerable<UserDTO>> GetUsersWithoutFriends(UserSearchParametersDto searchParameters);
    }
}
