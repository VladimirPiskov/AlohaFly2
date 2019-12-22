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
    public class PaymentService
    {
        private AlohaDb db;
        protected ILog log;

        public PaymentService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreatePayment(ServiceDataContracts.Payment payment)
        {
            try
            {
                var p = new Entities.Payment
                {
                    Code = payment.Code,
                    FiskalId = payment.FiskalId,
                    IsActive = payment.IsActive,
                    IsCash = payment.IsCash,
                    Name = payment.Name,
                    ToGo = payment.ToGo,
                     FRSend= payment.FRSend,
                     PaymentGroupId = payment.PaymentGroupId,
                     SHId =payment.SHId
                };

                db.Payments.Add(p);
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

        public OperationResultValue<ServiceDataContracts.Payment> GetPayment(long paymentId)
        {
            var payment = db.Payments.FirstOrDefault(p => p.Id == paymentId);
            var result = new OperationResultValue<ServiceDataContracts.Payment>()
            {
                Success = true,
                Result = new ServiceDataContracts.Payment
                {
                    Code = payment.Code,
                    FiskalId = payment.FiskalId,
                    Id = payment.Id,
                    IsActive = payment.IsActive,
                    IsCash = payment.IsCash,
                    Name = payment.Name,
                     ToGo = payment.ToGo,
                    FRSend = payment.FRSend,
                    SHId = payment.SHId,
                    PaymentGroupId= payment.PaymentGroupId,
                }
            };

            return result;
        }

        public OperationResult UpdatePayment(ServiceDataContracts.Payment payment)
        {
            var paymentToUpdate = db.Payments.FirstOrDefault(p => p.Id == payment.Id);

            if (paymentToUpdate == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Payment Not Found." };
            }

            paymentToUpdate.Code = payment.Code;
            paymentToUpdate.FiskalId = payment.FiskalId;
            paymentToUpdate.IsActive = payment.IsActive;
            paymentToUpdate.IsCash = payment.IsCash;
            paymentToUpdate.Name = payment.Name;

            paymentToUpdate.ToGo = payment.ToGo;
            paymentToUpdate.FRSend = payment.FRSend;
            paymentToUpdate.PaymentGroupId = payment.PaymentGroupId;
            paymentToUpdate.SHId = payment.SHId;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.Payment>> GetPaymentList()
        {
            var payments = db.Payments.ToList();
            var result = new OperationResultValue<List<ServiceDataContracts.Payment>>()
            {
                Success = true,
                Result = payments.Select(p => new ServiceDataContracts.Payment
                {
                    Code = p.Code,
                    FiskalId = p.FiskalId,
                    Id = p.Id,
                    IsActive = p.IsActive,
                    IsCash = p.IsCash,
                    Name = p.Name,
                     ToGo = p.ToGo,
                    FRSend = p.FRSend,
                    SHId =p.SHId,
                    PaymentGroupId = p.PaymentGroupId,
                }).ToList()
            };

            return result;
        }

        public OperationResult DeletePayment(long PaymentId)
        {
            var dp = db.Payments.FirstOrDefault(p => p.Id == PaymentId);
            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "PaymentNot Found." };
            }

            db.Payments.Remove(dp);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

    }
}