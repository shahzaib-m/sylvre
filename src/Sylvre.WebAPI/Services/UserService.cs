using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

using Microsoft.EntityFrameworkCore;

using Sylvre.WebAPI.Entities;
using Sylvre.WebAPI.Services.Exceptions;

namespace Sylvre.WebAPI.Services
{
    /// <summary>
    /// Specifies the methods to be implemented by the IUserService implementers.
    /// </summary>
    public interface IUserService
    {
        Task<User> CreateAsync(User newUser, string password);
        Task<User> RetrieveAsync(int id);
        Task<List<User>> RetrieveAllAsync();
        Task UpdateAsync(User updatedUser, User userToUpdate, string password = null);
        Task DeleteAsync(User userToDelete);

        Task<User> AuthenticateAsync(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly SylvreWebApiContext _context;
        
        public UserService(SylvreWebApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="newUser">The new user entity to create.</param>
        /// <param name="password">The password for the user.</param>
        /// <exception cref="UserServiceException">Throws when missing or invalid/reserved values are found.</exception>
        /// <returns>The created user.</returns>
        public async Task<User> CreateAsync(User newUser, string password)
        {
            // ensure username is sent
            if (string.IsNullOrWhiteSpace(newUser.Username))
                throw new UserServiceException("Username is required");

            // ensure password is sent
            if (string.IsNullOrWhiteSpace(password))
                throw new UserServiceException("Password is required");

            // ensure email is sent
            if (string.IsNullOrWhiteSpace(newUser.Email))
                throw new UserServiceException("Email is required");

            // ensure username doesn't already exist
            if (await _context.Users.AnyAsync(x => x.Username == newUser.Username))
                throw new UserServiceException($"Username '{newUser.Username}' is taken");

            // ensure email doesn't already exist
            if (await _context.Users.AnyAsync(x => x.Email == newUser.Email))
                throw new UserServiceException($"Email '{newUser.Email}' is taken");

            // generate hash and salt from the given password
            GenerateHashAndSaltFromString(password, out byte[] hash, out byte[] salt);
            newUser.PasswordHash = hash;
            newUser.PasswordSalt = salt;

            // add and save new user
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        /// <summary>
        /// Retrieves a user entity by the given id.
        /// </summary>
        /// <param name="id">The user id to look for.</param>
        /// <returns>The user entity if found, null otherwise.</returns>
        public Task<User> RetrieveAsync(int id)
        {
            // find and return a user by id
            return _context.Users.FindAsync(id);
        }
        /// <summary>
        /// Retrieves a list of all user entities.
        /// </summary>
        /// <returns>The list of all user entities</returns>
        public Task<List<User>> RetrieveAllAsync()
        {
            // return all users
            return _context.Users.ToListAsync();
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="updatedUser">The new updated user entity.</param>
        /// <param name="userToUpdate">The user entity to update.</param>
        /// <param name="password">The new password for the user (optional).</param>
        /// <exception cref="UserServiceException">Throws when invalid/reserved values are found.</exception>
        public async Task UpdateAsync(User updatedUser, User userToUpdate, string password = null)
        {
            _context.Users.Attach(userToUpdate);

            // update if new username is sent and not taken
            if (!string.IsNullOrWhiteSpace(updatedUser.Username)
                && updatedUser.Username != userToUpdate.Username)
            {
                if (await _context.Users.AnyAsync(x => x.Username == updatedUser.Username))
                    throw new UserServiceException($"Username '{updatedUser.Username}' is taken");

                userToUpdate.Username = updatedUser.Username;
            }

            // update if new email is sent and not taken
            if (!string.IsNullOrWhiteSpace(updatedUser.Email)
                && updatedUser.Email != userToUpdate.Email)
            {
                if (await _context.Users.AnyAsync(x => x.Email == updatedUser.Email))
                    throw new UserServiceException($"Email '{updatedUser.Email}' is taken");

                userToUpdate.Email = updatedUser.Email;
            }

            // update if new full name is sent
            if (!string.IsNullOrWhiteSpace(updatedUser.FullName))
                userToUpdate.FullName = updatedUser.FullName;

            // update if new password is sent
            if (!string.IsNullOrWhiteSpace(password))
            {
                GenerateHashAndSaltFromString(password, out byte[] hash, out byte[] salt);
                userToUpdate.PasswordHash = hash;
                userToUpdate.PasswordSalt = salt;
            }

            _context.Users.Update(userToUpdate);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete an existing user.
        /// </summary>
        /// <param name="userToDelete">The user entity to delete.</param>
        public async Task DeleteAsync(User userToDelete)
        {
            // delete user
            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();
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
            if (!IsStringValidAgainstHashAndSalt(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        /// <summary>
        /// Generates a hash and salt from a given string.
        /// </summary>
        /// <param name="str">The string to hash and salt.</param>
        /// <param name="hash">The out variable to set hash to.</param>
        /// <param name="salt">The out variable to set the salt to.</param>
        private static void GenerateHashAndSaltFromString(string str, 
            out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(str));
            }
        }
        /// <summary>
        /// Checks if a string is valid against a given hash and salt.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="hashToCheck">The hash to check against.</param>
        /// <param name="salt">The salt to use.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private static bool IsStringValidAgainstHashAndSalt(string str,
            byte[] hashToCheck, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var hmacComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(str));
                for (int i = 0; i < hmacComputedHash.Length; i++)
                {
                    if (hmacComputedHash[i] != hashToCheck[i])
                        return false;
                }

                return true;
            }
        }
    }
}
