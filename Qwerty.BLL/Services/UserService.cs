using Microsoft.AspNet.Identity;
using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.BLL.Services
{
    public class UserService : IUserService
    {
        public IUnitOfWork Database { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<OperationDetails> Create(UserDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByNameAsync(userDto.UserName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = userDto.UserName };
                var result = await Database.UserManager.CreateAsync(user, userDto.Password);
                if (result.Errors.Count() > 0)
                    return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
                await Database.UserManager.AddToRoleAsync(user.Id, userDto.Role);
                User Quser = new User() { ApplicationUser = user, Login = user.UserName, Password = userDto.Password, UserId = user.Id };
                UserProfile profile = new UserProfile() { UserId = Quser.UserId, User = Quser, Name = userDto.Name, Surname = userDto.Surname };
                Database.QUserManager.Create(Quser);
                Database.ProfileManager.Create(profile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Registration successful", "user");
            }
            else
            {
                return new OperationDetails(false, "User with this login already exists", "UserName");
            }
        }

        public async Task<ClaimsIdentity> Authenticate(UserDTO userDto)
        {
            ClaimsIdentity claim = null;
            ApplicationUser user = await Database.UserManager.FindAsync(userDto.UserName, userDto.Password);
            if (user != null)
                claim = await Database.UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
            return claim;
        }

        public async Task SetInitialData(UserDTO adminDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                var role = await Database.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole { Name = roleName };
                    await Database.RoleManager.CreateAsync(role);
                }
            }
            await Create(adminDto);
        }

        public async Task<UserDTO> FindUser(string UserName, string Password)
        {
            ApplicationUser user = await Database.UserManager.FindByNameAsync(UserName);
            if (user?.User.Password == Password)
            {
                UserProfile profile = user.User.UserProfile;
                return new UserDTO()
                {
                    Name = profile.Name,
                    Password = Password,
                    AboutUrl = profile.AboutUrl,
                    City = profile.City,
                    Country = profile.Country,
                    Email = profile.Email,
                    Id = user.Id,
                    ImageUrl = profile.ImageUrl,
                    Phone = profile.Phone,
                    Surname = profile.Surname,
                    UserName = UserName
                };
            }
            else return null;
        }

        public async Task<UserDTO> FindUserByUsername(string UserName)
        {
            ApplicationUser user = await Database.UserManager.FindByNameAsync(UserName);
            UserProfile profile = user.User.UserProfile;
            return new UserDTO()
            {
                Name = profile.Name,
                AboutUrl = profile.AboutUrl,
                City = profile.City,
                Country = profile.Country,
                Email = profile.Email,
                Id = user.Id,
                ImageUrl = profile.ImageUrl,
                Phone = profile.Phone,
                Surname = profile.Surname,
                UserName = UserName,
                Password = profile.User.Password
            };
        }

        public async Task<UserDTO> FindUserById(string UserId)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(UserId);
            UserProfile profile = user.User.UserProfile;
            return new UserDTO()
            {
                Name = profile.Name,
                AboutUrl = profile.AboutUrl,
                City = profile.City,
                Country = profile.Country,
                Email = profile.Email,
                Id = user.Id,
                ImageUrl = profile.ImageUrl,
                Phone = profile.Phone,
                Surname = profile.Surname,
                UserName = user.UserName,
                Password = profile.User.Password
            };
        }

        public async Task<OperationDetails> ChangeProfileInformation(UserDTO userDTO)
        {
            ApplicationUser user = await Database.UserManager.FindByNameAsync(userDTO.UserName);
            if (user != null)
            {
                UserProfile profile = user.User.UserProfile;
                profile.ImageUrl = userDTO.ImageUrl;
                profile.Name = userDTO.Name;
                profile.Surname = userDTO.Surname;
                profile.Phone = userDTO.Phone;
                profile.AboutUrl = userDTO.AboutUrl;
                profile.City = userDTO.City;
                profile.Country = userDTO.Country;
                profile.Email = userDTO.Email;
                Database.ProfileManager.Update(profile);
                await Database.SaveAsync();
                return new OperationDetails(true, "User successfully changed", "UserProfile");
            }
            else return new OperationDetails(false, "User is not found", "user");
        }

        public async Task<OperationDetails> DeleteUser(string UserId)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(UserId);
            if (user != null)
            {
                Database.UserManager.Delete(user);
                await Database.SaveAsync();
                return new OperationDetails(true, "User successfully deleted", "user");
            }
            else return new OperationDetails(false, "User is not found", "user");
        }

        public async Task<IEnumerable<UserDTO>> GetUsersByFullName(string Name, string Surname)
        {
            List<UserDTO> FindedUsers = null;
            await Task.Run(() =>
           {
               var profiles = Database.ProfileManager.GetAll().Where(x => x.Name == Name && x.Surname == Surname);
               if (profiles != null)
               {
                   FindedUsers = new List<UserDTO>();
                   foreach (var profile in profiles)
                   {
                       FindedUsers.Add(new UserDTO()
                       {
                           Name = profile.Name,
                           AboutUrl = profile.AboutUrl,
                           City = profile.City,
                           Country = profile.Country,
                           Email = profile.Email,
                           Id = profile.UserId,
                           ImageUrl = profile.ImageUrl,
                           Phone = profile.Phone,
                           Surname = profile.Surname,
                       });
                   }
               }
           });
            return FindedUsers;
        }
        public void Dispose()
        {
            Database.Dispose();
        }
        public async Task<OperationDetails> UploadImage(string ImageUrl, string UserName)
        {
            ApplicationUser user = await Database.UserManager.FindByNameAsync(UserName);
            if(user != null)
            {
                UserProfile profile = user.User.UserProfile;
                profile.ImageUrl = ImageUrl;
                Database.ProfileManager.Update(profile);
                await Database.SaveAsync();
                return new OperationDetails(true, ImageUrl, "profile");
            }
            else return new OperationDetails(false, "User is not found", "user");
        }
    }
}
