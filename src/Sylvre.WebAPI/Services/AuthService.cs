using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Sylvre.WebAPI.Utils;
using Sylvre.WebAPI.Entities;

namespace Sylvre.WebAPI.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string username, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly SylvreWebApiContext _context;

        public AuthService(SylvreWebApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Authenticates a user with the given username and password.
        /// </summary>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password to authenticate.</param>
        /// <returns>The authenticated user if authenticated, null otherwise.</returns>
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            // if username and/or password is empty, return null (auth denied)
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            // find the User by the given username
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

            // if username doesn't exist, return null (auth denied)
            if (user == null)
                return null;

            // check the given password against the stored hash and salt, return null if failed (auth denied)
            if (!HashUtils.IsStringValidAgainstHashAndSalt(password,
                    user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }
    }
}
