using Qwerty.BLL.DTO;
using Qwerty.BLL.Infrastructure;
using Qwerty.BLL.Interfaces;
using Qwerty.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Qwerty.DAL.EF;
using Qwerty.DAL.Identity;

namespace Qwerty.BLL.Services
{
    public class UserService : IUserService
    {
        //TODO create context for write and read
        private readonly ApplicationContext _appContext;
        private readonly ApplicationUserManager _userManager;

        public UserService(ApplicationContext appContext, ApplicationUserManager userManager)
        {
            _appContext = appContext;
            _userManager = userManager;
        }

        // TODO refactor this method 
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

            await _appContext.SaveChangesAsync();
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

        public async Task DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }

            var roles = await GetRolesByUserId(userId);
            if (roles.Contains(QwertyRoles.Blocked))
            {
                return;
            }

            await _userManager.AddToRoleAsync(user, QwertyRoles.Blocked);
        }

        public async Task RestoreAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }

            var roles = await GetRolesByUserId(userId);
            if (!roles.Contains(QwertyRoles.Blocked))
            {
                return;
            }

            await _userManager.RemoveFromRoleAsync(user, QwertyRoles.Blocked);
        }

        public async Task ChangeProfileInformation(UserDTO userDto)
        {
            var profile = await GetProfileAsync(u => u.Id == userDto.Id);
            if (profile == null)
            {
                throw new InvalidOperationException("This account should by already created");
            }

            profile.Name = userDto.Name;
            profile.Surname = userDto.Surname;
            profile.Phone = userDto.Phone;
            profile.AboutUrl = userDto.AboutUrl;
            profile.City = userDto.City;
            profile.Country = userDto.Country;
            profile.Email = userDto.Email;

            _appContext.Profiles.Update(profile);
            await _appContext.SaveChangesAsync();
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

        public async Task UploadImage(string imageUrl, string userName)
        {
            // TODO add azure blob storage
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesByUserId(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<UserDTO> FindAsync(string userName) =>
            await FindAsync(u => u.UserName == userName);

        public async Task<UserDTO> FindByIdAsync(string userId) =>
            await FindAsync(u => u.Id == userId);

        private async Task<UserDTO> FindAsync(Expression<Func<ApplicationUser, bool>> expression)
        {
            var profile = await GetProfileAsync(expression);

            return profile == null
                ? null
                : new UserDTO
                {
                    Name = profile.Name,
                    AboutUrl = profile.AboutUrl,
                    City = profile.City,
                    Country = profile.Country,
                    Email = profile.Email,
                    Id = profile.UserId,
                    ImageUrl = profile.ImageUrl,
                    Phone = profile.Phone,
                    Surname = profile.Surname
                };
        }

        private async Task<UserProfile> GetProfileAsync(Expression<Func<ApplicationUser, bool>> expression)
        {
            return await _appContext.Users
                .Where(expression)
                .Join(_appContext.Profiles, u => u.Id, p => p.UserId, (u, p) => p)
                .FirstOrDefaultAsync();
        }
    }
}