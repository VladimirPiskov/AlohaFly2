using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;
using log4net;
using AlohaService.Helpers;

namespace AlohaService.BusinessServices
{
    public class UserService
    {
        private AlohaDb db;
        protected ILog log;

        public UserService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateUser(UserInfo userInfo)
        {
            try
            {
                var dbContext = new AlohaDb();

                if (dbContext.Users.Any(usr => usr.UserName == userInfo.UserName || usr.Email == userInfo.Email))
                {
                    return new OperationResult
                    {
                        Success = false,
                        ErrorMessage = "User with such email or user name already exists. Please try another email and user name."
                    };
                }

                var user = new Entities.User();
                user.FullName = userInfo.FullName;
                user.Email = userInfo.Email;
                var password = PasswordHelper.GetHashedPassword(userInfo.Password);
                user.Password = password.Password;
                user.PasswordSalt = password.PasswordSalt;
                user.Phone = userInfo.Phone;
                user.RegistrationDateTime = DateTime.Now;
                user.SequrityAnswer = userInfo.SequrityAnswer;
                user.SequrityQuestion = userInfo.SequrityQuestion;
                user.UserName = userInfo.UserName;
                user.UserRole = (int)userInfo.UserRole;
                user.IsActive = userInfo.IsActive;


                user.UpdatedDate = DateTime.Now;
                //user.LastUpdatedSession = userInfo.LastUpdatedSession;

                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = user.Id
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<ServiceDataContracts.User> GetUser(long userId)
        {
            var usr = db.Users.FirstOrDefault(user => user.Id == userId);
            var result = new OperationResultValue<ServiceDataContracts.User>();
            result.Success = true;
            result.Result = new ServiceDataContracts.User();
            result.Result.Email = usr.Email;
            result.Result.FullName = usr.FullName;
            result.Result.Id = usr.Id;
            result.Result.Phone = usr.Phone;
            result.Result.RegistrationStatus = usr.RegistrationStatus;
            result.Result.SequrityAnswer = usr.SequrityAnswer;
            result.Result.SequrityQuestion = usr.SequrityQuestion;
            result.Result.UserName = usr.UserName;
            result.Result.UserRole = (UserRole)usr.UserRole;
            result.Result.IsActive = usr.IsActive;

            return result;
        }

        public OperationResult UpdateUser(ServiceDataContracts.User user)
        {
            var usr = db.Users.FirstOrDefault(u => u.Id == user.Id);
            if (usr == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "User Not Found." };
            }
            usr.Email = user.Email;
            usr.FullName = user.FullName;
            usr.Phone = user.Phone;
            usr.RegistrationStatus = user.RegistrationStatus;
            usr.SequrityAnswer = user.SequrityAnswer;
            usr.SequrityQuestion = user.SequrityQuestion;
            usr.IsActive = user.IsActive;

            var password = PasswordHelper.GetHashedPassword(user.Password);
            usr.Password = password.Password;
            usr.PasswordSalt = password.PasswordSalt;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteUser(long userId)
        {
            var usr = db.Users.FirstOrDefault(user => user.Id == userId);
            if(usr == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "User Not Found." };
            }

            db.Users.Remove(usr);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<ServiceDataContracts.User> LoginUser(string loginOrEmail, string password)
        {
            var usr = db.Users.FirstOrDefault(user => user.UserName == loginOrEmail || user.Email == loginOrEmail);
            if (usr == null)
            {
                return new OperationResultValue<ServiceDataContracts.User> { Success = false, ErrorMessage = "User Not Found." };
            }

            if(PasswordHelper.PasswordsEqual(usr.Password, usr.PasswordSalt, password))
            {
                var result = new ServiceDataContracts.User();
                result.Id = usr.Id;
                result.Email = usr.Email;
                result.FullName = usr.FullName;
                result.Phone = usr.Phone;
                result.RegistrationStatus = usr.RegistrationStatus;
                result.SequrityAnswer = usr.SequrityAnswer;
                result.SequrityQuestion = usr.SequrityQuestion;
                result.UserName = usr.UserName;
                result.UserRole = (UserRole)usr.UserRole;
                result.IsActive = usr.IsActive;
                return new OperationResultValue<ServiceDataContracts.User> { Success = true, Result = result };
            }
            else
            {
                return new OperationResultValue<ServiceDataContracts.User> { Success = false, ErrorMessage = "Password is wrong." };
            }
        }

        public OperationResultValue<List<ServiceDataContracts.User>> GetUserList()
        {
            var list = db.Users.ToList();
            var result = list.Select(usr => new ServiceDataContracts.User {
                Id = usr.Id,
                Email = usr.Email,
                FullName = usr.FullName,
                Phone = usr.Phone,
                RegistrationStatus = usr.RegistrationStatus,
                SequrityAnswer = usr.SequrityAnswer,
                SequrityQuestion = usr.SequrityQuestion,
                UserName = usr.UserName,
                UserRole = (UserRole)usr.UserRole,
                IsActive = usr.IsActive
        }
            ).ToList();
            return new OperationResultValue<List<ServiceDataContracts.User>> { Success = true, Result = result };
        }
    }
}