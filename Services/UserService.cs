using System.Collections.Generic;
using System.Linq;

using KM.GD.PrintInvoices.Models.Users;
using KM.GD.PrintInvoices.Entities;
using KM.GD.PrintInvoices.Helpers;
using KM.GD.PrintInvoices.Models;

namespace KM.GD.PrintInvoices.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
        void Register(RegisterRequest model);
        void Update(int id, UpdateRequest model);
        void Delete(int id);
    }

    public class UserService : IUserService
    {
        private UserContext _context;


        public UserService(UserContext context)
        {
            _context = context;
        
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            User user = _context.Users.SingleOrDefault(x =>  x.Username.Replace(@"\\", @"\") == model.Username);
        // user = _context.Users.FirstOrDefault();
            // validate
            if (user == null )
            throw new AppException("Username or password is incorrect");
       
         // authentication successful
         AuthenticateResponse response = new AuthenticateResponse();
         response.Id = user.UserId;
         response.FirstName = user.FirstName;
         response.LastName = user.LastName;
         response.Username = user.Username;
         response.Token = "my-user-token";
            return response;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return getUser(id);
        }

        public void Register(RegisterRequest model)
        {
            // validate
            if (_context.Users.Any(x => x.Username == model.Username))
                throw new AppException("Lo Username '" + model.Username + "' esiste già.");

         // map model to new user object
         User user = new User();
         user.LastName=  model.LastName;
         user.FirstName = model.FirstName;
         user.Username = model.Username;
         user.UpdatedBy = "Admin";
           

            // save user
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(int id, UpdateRequest model)
        {
            var user = getUser(id);

            // validate
            if (model.Username != user.Username && _context.Users.Any(x => x.Username == model.Username))
                throw new AppException("Lo Username '" + model.Username + "' esiste già.");

         // hash password if it was entered
         //if (!string.IsNullOrEmpty(model.Password))
         //    user.PasswordHash = BCryptNet.HashPassword(model.Password);

         // copy model to user and save
         user.FirstName = model.FirstName;
         user.LastName = model.LastName;
         user.Username = model.Username;
         user.UpdatedBy = "Admin";
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = getUser(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        // helper methods

        private User getUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
    }
}