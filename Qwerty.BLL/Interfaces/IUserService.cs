using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qwerty.BLL.Interfaces
{
    public interface IUserService 
    {
        Task<OperationDetails> CreateUserAsync(UserDTO userDto);
        Task<UserDTO> FindUserAsync(string UserName, string Password);
        Task<OperationDetails> ChangeProfileInformation(UserDTO userDTO);
        Task<IEnumerable<UserDTO>> GetUsers(string Name = null, string Surname = null, string Country = null, string City = null);
        UserDTO FindUserByUsername(string UserName);
        Task<OperationDetails> UploadImage(string ImageUrl, string UserName);
        Task<UserDTO> FindUserByIdAsync(string UserId);
        Task<IList<string>> GetRolesByUserId(string id);
        Task<OperationDetails> DeleteUser(string userId);
        Task<OperationDetails> RestoreAccount(string userId);
        Task<IEnumerable<UserDTO>> GetUsersWithoutFriends(UserSearchParametersDto searchParameters);
    }
}
