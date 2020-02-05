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
    public class PaymentGroupService
    {
        private AlohaDb db;
        protected ILog log;

        public PaymentGroupService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreatePaymentGroup(ServiceDataContracts.PaymentGroup payment)
        {
            try
            {
                var p = new Entities.PaymentGroup
                {
                    Code = payment.Code,
                    IsActive = payment.IsActive,
                    Sale = payment.Sale,
                    Name = payment.Name,
                };

                p.UpdatedDate = DateTime.Now;
                p.LastUpdatedSession = payment.LastUpdatedSession;

                db.PaymentGroups.Add(p);
                db.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = p.Id
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

        public OperationResultValue<ServiceDataContracts.PaymentGroup> GetPaymentGroup(long paymentId)
        {
            var payment = db.PaymentGroups.FirstOrDefault(p => p.Id == paymentId);
            var result = new OperationResultValue<ServiceDataContracts.PaymentGroup>()
            {
                Success = true,
                Result = new ServiceDataContracts.PaymentGroup
                {
                    Code = payment.Code,
                    
                    Id = payment.Id,
                    IsActive = payment.IsActive,
                    Sale = payment.Sale,
                    Name = payment.Name,
                    
                }
            };

            return result;
        }

        public OperationResult UpdatePaymentGroup(ServiceDataContracts.PaymentGroup payment)
        {
            var paymentToUpdate = db.PaymentGroups.FirstOrDefault(p => p.Id == payment.Id);

            if (paymentToUpdate == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "PaymentGroup Not Found." };
            }

            paymentToUpdate.Code = payment.Code;
            
            paymentToUpdate.IsActive = payment.IsActive;
            
            paymentToUpdate.Name = payment.Name;
            paymentToUpdate.Sale = payment.Sale;

            paymentToUpdate.UpdatedDate = DateTime.Now;
            paymentToUpdate.LastUpdatedSession = payment.LastUpdatedSession;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.PaymentGroup>> GetPaymentGroupList()
        {
            var payments = db.PaymentGroups.ToList();
            var result = new OperationResultValue<List<ServiceDataContracts.PaymentGroup>>()
            {
                Success = true,
                Result = payments.Select(p => new ServiceDataContracts.PaymentGroup
                {
                    Code = p.Code,
                    
                    Id = p.Id,
                    IsActive = p.IsActive,
                    
                    Name = p.Name,
                    Sale = p.Sale
                }).ToList()
            };

            return result;
        }


        public OperationResult DeletePaymentGroup(long PaymentGroupId)
        {
            var dp = db.PaymentGroups.FirstOrDefault(p => p.Id == PaymentGroupId);
            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "MarketingChannelNot Found." };
            }

            db.PaymentGroups.Remove(dp);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

    }
}