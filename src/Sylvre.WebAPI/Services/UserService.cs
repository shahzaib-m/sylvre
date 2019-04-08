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
        Task UpdateAsync(User updatedUser, User userToUpdate);
        Task DeleteAsync(User userToDelete);

        Task ChangePassword(string newPassword, User userToUpdate);

        Task<bool> IsUsernameTaken(string username);
        Task<bool> IsEmailTaken(string email);
        Task<bool> IsCorrectPasswordForUserId(string password, int userId);
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
        /// <exception cref="UserServiceException">Throws when invalid/reserved values are found.</exception>
        public async Task UpdateAsync(User updatedUser, User userToUpdate)
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
        /// Updates an existing user's password.
        /// </summary>
        /// <param name="newPassword">The new password to set.</param>
        /// <param name="userToUpdate">The user to update the password for.</param>
        public async Task ChangePassword(string newPassword, User userToUpdate)
        {
            _context.Users.Attach(userToUpdate);

            HashUtils.GenerateHashAndSaltFromString(newPassword, out byte[] hash, out byte[] salt);
            userToUpdate.PasswordHash = hash;
            userToUpdate.PasswordSalt = salt;

            _context.Users.Update(userToUpdate);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Checks whether a username is taken by an existing user. 
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>True if taken, false otherwise.</returns>
        public Task<bool> IsUsernameTaken(string username)
        {
            return _context.Users.AnyAsync(x => x.Username == username);
        }

        /// <summary>
        /// Checks whether an email is taken by an existing user. 
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>True if taken, false otherwise.</returns>
        public Task<bool> IsEmailTaken(string email)
        {
            return _context.Users.AnyAsync(x => x.Email == email);
        }

        /// <summary>
        /// Checks whether a given password is the correct password the given user id.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="userId">The user id the password belongs to.</param>
        /// <returns>True if correct, false otherwise.</returns>
        public async Task<bool> IsCorrectPasswordForUserId(string password, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return HashUtils.IsStringValidAgainstHashAndSalt(password,
                user.PasswordHash, user.PasswordSalt);
        }
    }
}
