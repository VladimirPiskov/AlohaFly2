using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;

namespace AlohaService.BusinessServices
{
    public class AlertService
    {
        private AlohaDb db;

        public AlertService(AlohaDb databaseContext)
        {
            db = databaseContext;
        }

        public long CreateAlert(ServiceDataContracts.Alert alert)
        {
            var a = new Entities.Alert();
            a.End = alert.End;
            a.Message = alert.Message;
            a.Period = alert.Period;
            a.Start = alert.Start;

            db.Alerts.Add(a);
            db.SaveChanges();

            return a.Id;
        }

        public ServiceDataContracts.Alert GetAlert(long alertId)
        {
            var alert = db.Alerts.FirstOrDefault(a => a.Id == alertId);
            var result = new ServiceDataContracts.Alert();
            result.End = alert.End;
            result.Id = alert.Id;
            result.Message = alert.Message;
            result.Period = alert.Period;
            result.Start = alert.Start;

            return result;
        }

        public void UpdateAlert(ServiceDataContracts.Alert alert)
        {
            var alertToUpdate = db.Alerts.FirstOrDefault(a => a.Id == alert.Id);
            alertToUpdate.End = alert.End;
            alertToUpdate.Message = alert.Message;
            alertToUpdate.Period = alert.Period;
            alertToUpdate.Start = alert.Start;

            db.SaveChanges();
        }

        public void DeleteAlert(long alertId)
        {
            var alert = db.Alerts.FirstOrDefault(a => a.Id == alertId);
            db.Alerts.Remove(alert);
            db.SaveChanges();
        }

        public List<ServiceDataContracts.Alert> GetAlertList()
        {
            return db.Alerts.ToList().Select(
                a => new ServiceDataContracts.Alert
                {
                    End = a.End,
                    Id = a.Id,
                    Message = a.Message,
                    Period = a.Period,
                    Start = a.Start
                }).ToList();
        }

        public List<ServiceDataContracts.Alert> GetAlertListByDateRange(DateTime from, DateTime to)
        {
            return db.Alerts.Where(a => (a.Start >= from && a.Start <= to) || (a.End >= from && a.End <= to)).ToList().Select(
                a => new ServiceDataContracts.Alert
                {
                    End = a.End,
                    Id = a.Id,
                    Message = a.Message,
                    Period = a.Period,
                    Start = a.Start
                }).ToList();
        }

    }
}