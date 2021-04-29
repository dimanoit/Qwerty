using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.DAL.Entities;
using Qwerty.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Qwerty.DAL.EF;
using Qwerty.DAL.Identity;

namespace Qwerty.BLL.Services
{
    public class UserService : IUserService, IDisposable
    {
        public IUnitOfWork Database { get; set; }

        //TODO create context for write and read
        private readonly ApplicationContext _appContext;
        private readonly ApplicationUserManager _userManager;

        public UserService(IUnitOfWork uow, ApplicationContext appContext, ApplicationUserManager userManager)
        {
            Database = uow;
            _appContext = appContext;
            _userManager = userManager;
        }

        public async Task CreateAsync(UserDTO userDto, string password)
        {
            if (userDto == null)
            {
                // TODO Integrate Fluent Validation to validate DTOs 
                throw new ValidationException("UserDTO was null", "");
            }

            var user = await _userManager.FindByNameAsync(userDto.UserName);
            if (user != null)
            {
                // TODO Create Domain Entity with all validation logic(made user processing through DDD concept)
                throw new ValidationException("User with this login already exists", userDto.UserName);
            }

            user = new ApplicationUser
            {
                UserName = userDto.UserName
            };

            // TODO make user creation process in single transaction 
            var result = await _userManager.CreateAsync(user, password);

            if (result.Errors.Any())
            {
                var errorsDescriptions = result.Errors.Select(e => e.Description);
                var joinedError = string.Join(',', errorsDescriptions);
                throw new ValidationException(joinedError);
            }

            if (userDto.Roles.Any())
            {
                foreach (var role in userDto.Roles)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            var qUser = new User
            {
                ApplicationUser = user,
                Login = user.UserName,
                UserId = user.Id
            };

            var profile = new UserProfile
            {
                UserId = qUser.UserId,
                User = qUser,
                Name = userDto.Name,
                Surname = userDto.Surname
            };

            _appContext.Profiles.Add(profile);
            _appContext.QUsers.Add(qUser);

            await Database.SaveAsync();
        }

        public async Task<UserDTO> LoginAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new ValidationException($"User with login - {userName} doesn't exist");
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordCorrect)
            {
                throw new ValidationException("Password incorrect");
            }

            var userProfile = await _appContext.Profiles.FirstAsync(u => u.UserId == user.Id);
            return new UserDTO
            {
                Name = userProfile.Name,
                AboutUrl = userProfile.AboutUrl,
                City = userProfile.City,
                Country = userProfile.Country,
                Email = userProfile.Email,
                Id = user.Id,
                ImageUrl = userProfile.ImageUrl,
                Phone = userProfile.Phone,
                Surname = userProfile.Surname,
                UserName = userName
            };
        }

        public UserDTO FindUserByUsername(string UserName)
        {
            User qUser = Database.QUserManager.GetAll().Where(x => x.ApplicationUser.UserName == UserName)
                .FirstOrDefault();
            ApplicationUser user = qUser.ApplicationUser;
            if (user == null) throw new ValidationException("User is not found", UserName);
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
            };
        }

        public async Task<UserDTO> FindUserByIdAsync(string UserId)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(UserId);
            if (user == null) throw new ValidationException("User is not found", UserId);
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
            };
        }

        public async Task DeleteUser(string userId)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(userId);
            if (user == null) throw new ValidationException("User not found", "");
            var UserRoles = this.GetRolesByUserId(userId);
            if (UserRoles.Result.Contains("deleted") == false)
            {
                await Database.UserManager.AddToRoleAsync(user, "deleted");
            }
            else throw new ValidationException("User already delted", "");

            await Database.SaveAsync();
        }

        public async Task RestoreAccount(string userId)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(userId);
            if (user == null) throw new ValidationException("User not found", "");
            var UserRoles = this.GetRolesByUserId(userId);
            if (UserRoles.Result.Contains("deleted") == true)
            {
                await Database.UserManager.RemoveFromRoleAsync(user, "deleted");
            }
            else throw new ValidationException("User not delted", "");

            await Database.SaveAsync();
        }


        public async Task ChangeProfileInformation(UserDTO userDTO)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(userDTO.Id);
            if (user == null) throw new ValidationException("User is not found", userDTO.Id);
            UserProfile profile = user.User.UserProfile;
            profile.Name = userDTO.Name;
            profile.Surname = userDTO.Surname;
            profile.Phone = userDTO.Phone;
            profile.AboutUrl = userDTO.AboutUrl;
            profile.City = userDTO.City;
            profile.Country = userDTO.Country;
            profile.Email = userDTO.Email;
            Database.ProfileManager.Update(profile);
            await Database.SaveAsync();
        }


        public async Task<IEnumerable<UserDTO>> GetUsers(string Name = null, string Surname = null,
            string Country = null, string City = null)
        {
            List<UserDTO> FindedUsers = null;
            var profiles = Database.ProfileManager.GetAll();
            if (Name != null) profiles = profiles.Where(x => x.Name.Contains(Name));
            if (Surname != null) profiles = profiles.Where(x => x.Surname.Contains(Surname));
            if (Country != null) profiles = profiles.Where(x => x.Country == Country);
            if (City != null) profiles = profiles.Where(x => x.City == City);
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
                        Roles = GetRolesByUserId(profile.UserId).Result.ToArray()
                    });
                }
            }

            return FindedUsers;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersWithoutFriends(UserSearchParametersDto searchParameters)
        {
            var baseQuery = _appContext.Profiles.AsQueryable();
            if (!string.IsNullOrEmpty(searchParameters.City))
            {
                baseQuery = baseQuery.Where(p => p.City.ToUpper() == searchParameters.City.ToUpper());
            }

            if (!string.IsNullOrEmpty(searchParameters.Country))
            {
                baseQuery = baseQuery.Where(p => p.Country.ToUpper() == searchParameters.Country.ToUpper());
            }

            if (!string.IsNullOrEmpty(searchParameters.Name))
            {
                baseQuery = baseQuery.Where(p => p.Name.ToUpper().Contains(searchParameters.Name.ToUpper()));
            }

            if (!string.IsNullOrEmpty(searchParameters.Surname))
            {
                baseQuery = baseQuery.Where(p => p.Country.ToUpper().Contains(searchParameters.Country.ToUpper()));
            }

            //TODO refactor little bit
            var usersWithoutFriends = await baseQuery
                .Where(p => p.UserId != searchParameters.CurrentUserId)
                .Where(p =>
                    !_appContext.UserFriends
                        .Where(uf => uf.UserId == searchParameters.CurrentUserId)
                        .Select(uf => uf.FriendId)
                        .Contains(p.UserId)
                )
                .Where(p =>
                    !_appContext.UserFriends
                        .Where(uf => uf.FriendId == searchParameters.CurrentUserId)
                        .Select(uf => uf.UserId)
                        .Contains(p.UserId))
                .ToListAsync();

            // TODO add automapper config
            return usersWithoutFriends?.Select(profile => new UserDTO
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
                Roles = GetRolesByUserId(profile.UserId).Result.ToArray()
            });
        }


        public async Task UploadImage(string ImageUrl, string UserName)
        {
            ApplicationUser user = await Database.UserManager.FindByNameAsync(UserName);
            if (user == null) throw new ValidationException("User is not found", UserName);
            UserProfile profile = user.User.UserProfile;
            profile.ImageUrl = ImageUrl;
            Database.ProfileManager.Update(profile);
            await Database.SaveAsync();
        }

        public async Task<IList<string>> GetRolesByUserId(string id)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(id);
            return await Database.UserManager.GetRolesAsync(user);
        }


        public void Dispose()
        {
            Database.Dispose();
        }
    }
}