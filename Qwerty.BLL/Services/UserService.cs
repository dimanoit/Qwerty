﻿using Microsoft.AspNet.Identity;
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
                user = new ApplicationUser { UserName = userDto.UserName};
                var result = await Database.UserManager.CreateAsync(user, userDto.Password);
                if (result.Errors.Count() > 0)
                    return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
                await Database.UserManager.AddToRoleAsync(user.Id, userDto.Role);
                User Quser = new User() { ApplicationUser = user, Login = user.UserName, Password = userDto.Password, UserId = user.Id };
                UserProfile profile = new UserProfile() { UserId = Quser.UserId, User = Quser, Name = userDto.Name, Surname = userDto.Surname };
                Database.QUserManager.Create(Quser);
                Database.ProfileManager.Create(profile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Registration successful", "UserName");
            }
            else
            {
                return new OperationDetails(false, "User with this login already exists", "Email");
            }
        }

        public async Task<ClaimsIdentity> Authenticate(UserDTO userDto)
        {
            ClaimsIdentity claim = null;
            // находим пользователя
            ApplicationUser user = await Database.UserManager.FindAsync(userDto.Email, userDto.Password);
            // авторизуем его и возвращаем объект ClaimsIdentity
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

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
