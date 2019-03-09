using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using Sylvre.WebAPI.Utils;
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
            HashUtils.GenerateHashAndSaltFromString(password, out byte[] hash, out byte[] salt);
            newUser.PasswordHash = hash;
            newUser.PasswordSalt = salt;

            // standard user by default
            newUser.IsAdmin = false;

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
                HashUtils.GenerateHashAndSaltFromString(password, out byte[] hash, out byte[] salt);
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
    }
}
