using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;

namespace AlohaService.BusinessServices
{
    public class LogItemService
    {
        private AlohaDb db;

        public LogItemService(AlohaDb databaseContext)
        {
            db = databaseContext;
        }

        public OperationResultValue<List<ServiceDataContracts.LogItem>> GetLogItems(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var query = db.LogItems.Where(o => true);

                if (dateFrom != null)
                {
                    query = query.Where(o => o.CreationDate >= dateFrom);
                }

                if (dateTo != null)
                {
                    query = query.Where(o => o.CreationDate <= dateTo);
                }

                var result = new OperationResultValue<List<ServiceDataContracts.LogItem>>();

                result.Result = query.Select(log =>
                       new ServiceDataContracts.LogItem
                       {
                           ActionDescription = log.ActionDescription,
                           CreatedBy = new ServiceDataContracts.User
                           {
                               Email = log.CreatedBy.Email,
                               Id = log.CreatedBy.Id,
                               FullName = log.CreatedBy.FullName,
                               IsActive = log.CreatedBy.IsActive,
                               Phone = log.CreatedBy.Phone,
                               UserName = log.CreatedBy.UserName,
                               UserRole = (UserRole)log.CreatedBy.UserRole
                           },
                           ActionName = log.ActionName,
                           CreationDate = log.CreationDate,
                           Id = log.Id,
                           MethodName = log.MethodName,
                           StateAfter = log.StateAfter,
                           StateBefore = log.StateBefore

                       }).ToList();

                result.Success = true;
                return result;
            }
            catch (Exception e)
            {
                return new OperationResultValue<List<ServiceDataContracts.LogItem>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

    }
}