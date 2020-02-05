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
    public class ContactPersonService
    {
        private AlohaDb db;
        protected ILog log;

        public ContactPersonService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateContactPerson(ServiceDataContracts.ContactPerson contactPerson)
        {
            try
            {
                var dbContext = new AlohaDb();

                var cp = new Entities.ContactPerson();
                cp.FirstName = contactPerson.FirstName;
                cp.SecondName = contactPerson.SecondName;
                cp.Phone = contactPerson.Phone;
                cp.IsActive = contactPerson.IsActive;
                cp.Email = contactPerson.Email;

                cp.UpdatedDate = DateTime.Now;
                cp.LastUpdatedSession = contactPerson.LastUpdatedSession;


                dbContext.ContactPersons.Add(cp);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = cp.Id
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

        public OperationResultValue<List<ServiceDataContracts.ContactPerson>> GetContactPersonList()
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.ContactPersons.Select(cp => new ServiceDataContracts.ContactPerson
                {
                    Id = cp.Id,
                    FirstName = cp.FirstName,
                    SecondName = cp.SecondName,
                    Phone = cp.Phone,
                    IsActive = cp.IsActive,
                    Email = cp.Email
                }).ToList();
                return new OperationResultValue <List<ServiceDataContracts.ContactPerson>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue <List<ServiceDataContracts.ContactPerson>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<ServiceDataContracts.ContactPerson> GetContactPerson(long contactPersonId)
        {
            var c = db.ContactPersons.FirstOrDefault(cp => cp.Id == contactPersonId);
            var result = new OperationResultValue<ServiceDataContracts.ContactPerson>();
            result.Success = true;
            result.Result = new ServiceDataContracts.ContactPerson();
            result.Result.FirstName = c.FirstName;
            result.Result.SecondName = c.SecondName;
            result.Result.Id = c.Id;
            result.Result.Phone = c.Phone;
            result.Result.IsActive = c.IsActive;
            result.Result.Email = c.Email;

            return result;
        }

        public OperationResult UpdateContactPerson(ServiceDataContracts.ContactPerson contactPerson)
        {
            var c = db.ContactPersons.FirstOrDefault(cp => cp.Id == contactPerson.Id);

            if (c == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Contact Person Not Found." };
            }

            c.FirstName = contactPerson.FirstName;
            c.SecondName = contactPerson.SecondName;
            c.Phone = contactPerson.Phone;
            c.IsActive = contactPerson.IsActive;
            c.Email = contactPerson.Email;

            c.UpdatedDate = DateTime.Now;
            c.LastUpdatedSession = contactPerson.LastUpdatedSession;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteContactPerson(long contactPersonId)
        {
            var cp = db.ContactPersons.FirstOrDefault(c => c.Id == contactPersonId);
            if (cp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Contact Person Not Found." };
            }

            db.ContactPersons.Remove(cp);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

    }
}