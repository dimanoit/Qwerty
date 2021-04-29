using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Qwerty.BLL.Interfaces;
using Qwerty.WebApi.Configurations;
using Qwerty.WebApi.Models;
using Serilog;

namespace Qwerty.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {

        private IUserService _userService { get; set; }

        public AuthorizationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("token")]
        public async Task<ActionResult> Token(LoginViewModel person)
        {
            var identity = await GetIdentity(person.Username, person.Password);
            if (identity == null)
            {
                Log.Warning($"User {person.Username} don`t exist in database");
                return BadRequest("User don`t exist");
            }
            var result = GenerateToken(identity);
            string userId = string.Empty;
            var roles = new List<string>();
            foreach (var claim in identity.Claims)
            {
                if (claim.Type == ClaimsIdentity.DefaultNameClaimType) userId = claim.Value;
                if (claim.Type == ClaimsIdentity.DefaultRoleClaimType) roles.Add(claim.Value);
            }
            if (result == null)
            {
                Log.Warning($"User {person.Username} don`t exist in database or don`t have claims");
                return NotFound();
            }
            else
            {
                return Ok(new { result, userId, roles });
            }
        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var user = await _userService.LoginAsync(username, password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType , user.Id),
                };

                var userRoles = await _userService.GetRolesByUserId(user.Id);
                foreach (string roleName in userRoles)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, roleName));
                }
                var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return null;
        }

        private string GenerateToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}