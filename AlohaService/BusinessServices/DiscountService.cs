using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;

namespace AlohaService.BusinessServices
{
    public class DiscountService
    {
        private AlohaDb db;

        public DiscountService(AlohaDb databaseContext)
        {
            db = databaseContext;
        }

        public long CreateDiscountRange(ServiceDataContracts.DiscountRange discountRange)
        {
            var dr = new Entities.DiscountRange();
            dr.DiscountPercent = discountRange.DiscountPercent;
            dr.End = discountRange.End;
            dr.Start = discountRange.Start;



            db.DiscountRanges.Add(dr);
            db.SaveChanges();

            return dr.Id;
        }

        public ServiceDataContracts.DiscountRange GetDiscountRange(long discountRangeId)
        {
            var dr = db.DiscountRanges.FirstOrDefault(ac => ac.Id == discountRangeId);
            var result = new ServiceDataContracts.DiscountRange();

            result.DiscountPercent = dr.DiscountPercent;
            result.End = dr.End;
            result.Id = dr.Id;
            result.Start = dr.Start;

            return result;
        }

        public void UpdateDiscountRange(ServiceDataContracts.DiscountRange discountRange)
        {
            var drToUpdate = db.DiscountRanges.FirstOrDefault(dr => dr.Id == discountRange.Id);
            drToUpdate.DiscountPercent = discountRange.DiscountPercent;
            drToUpdate.End = discountRange.End;
            drToUpdate.Start = discountRange.Start;

            db.SaveChanges();
        }

        public void DeleteDiscountRange(long discountRangeId)
        {
            var discountRange = db.DiscountRanges.FirstOrDefault(dr => dr.Id == discountRangeId);
            db.DiscountRanges.Remove(discountRange);
            db.SaveChanges();
        }


        public List<ServiceDataContracts.DiscountRange> GetDiscountRangesList()
        {
            return db.DiscountRanges.ToList().Select(
                dr => new ServiceDataContracts.DiscountRange
                {
                    DiscountPercent = dr.DiscountPercent,
                    End = dr.End,
                    Id = dr.Id,
                    Start = dr.Start
                }).ToList();
        }

        public long CreateDiscount(ServiceDataContracts.Discount discount)
        {
            var d = new Entities.Discount();
            d.Name = discount.Name;
            d.ToGo = discount.ToGo;

            d.UpdatedDate = DateTime.Now;
            d.LastUpdatedSession = discount.LastUpdatedSession;

            db.Discounts.Add(d);
            db.SaveChanges();

            return d.Id;
        }

        public ServiceDataContracts.Discount GetDiscount(long discountId)
        {
            var d = db.Discounts.FirstOrDefault(ac => ac.Id == discountId);
            var result = new ServiceDataContracts.Discount();

            result.Name = d.Name;
            result.ToGo = d.ToGo;
            result.Id = d.Id;

            var links = db.DiscountDiscountRangeLinks.Where(l => l.DiscountId == discountId);
            result.Ranges = new List<ServiceDataContracts.DiscountRange>();

            foreach(var link in links)
            {
                var r = db.DiscountRanges.First(dr => dr.Id == link.DiscountRangeId);
                result.Ranges.Add(new ServiceDataContracts.DiscountRange
                {
                    DiscountPercent = r.DiscountPercent,
                    End = r.End,
                    Id = r.Id,
                    Start = r.Start
                });
            }

            return result;
        }

        public void UpdateDiscount(ServiceDataContracts.Discount discount)
        {
            var dToUpdate = db.Discounts.FirstOrDefault(d => d.Id == discount.Id);
            dToUpdate.Name = discount.Name;
            dToUpdate.ToGo = discount.ToGo;

            dToUpdate.UpdatedDate = DateTime.Now;
            dToUpdate.LastUpdatedSession = discount.LastUpdatedSession;

            db.SaveChanges();
        }

        public void DeleteDiscount(long discountId)
        {
            var discount = db.Discounts.FirstOrDefault(d => d.Id == discountId);

            var links = db.DiscountDiscountRangeLinks.Where(l => l.DiscountId == discountId);
            db.DiscountDiscountRangeLinks.RemoveRange(links);

            db.Discounts.Remove(discount);
            db.SaveChanges();
        }

        public List<ServiceDataContracts.Discount> GetDiscountList()
        {
            var result = db.Discounts.ToList().Select(
                d => new ServiceDataContracts.Discount
                {
                    Name = d.Name,
                    Id = d.Id,
                    ToGo = d.ToGo
                }).ToList();

            foreach(var discount in result.ToList())
            {
                var links = db.DiscountDiscountRangeLinks.Where(l => l.DiscountId == discount.Id).ToList();
                discount.Ranges = new List<ServiceDataContracts.DiscountRange>();

                foreach (var link in links)
                {
                    var r = db.DiscountRanges.First(dr => dr.Id == link.DiscountRangeId);
                    result.First(ds => ds.Id == discount.Id).Ranges.Add(new ServiceDataContracts.DiscountRange
                    {
                        
                        DiscountPercent = r.DiscountPercent,
                        End = r.End,
                        Id = r.Id,
                        Start = r.Start
                    });
                }
            }

            return result;
        }

        public void AddRangeToDiscount(long discountId, long discountRangeId)
        {
            var discount = db.Discounts.First(d => d.Id == discountId);
            var range = db.DiscountRanges.First(dr => dr.Id == discountRangeId);

            db.DiscountDiscountRangeLinks.Add(new DiscountDiscountRangeLink
            {
                DiscountId = discountId,
                DiscountRangeId = discountRangeId
            });

            db.SaveChanges();
        }

        public void RemoveRangeFromDiscount(long discountId, long discountRangeId)
        {
            var discount = db.Discounts.First(d => d.Id == discountId);
            var range = db.DiscountRanges.First(dr => dr.Id == discountRangeId);

            var link = db.DiscountDiscountRangeLinks.First(l => l.DiscountId == discountId && l.DiscountRangeId == discountRangeId);

            db.DiscountDiscountRangeLinks.Remove(link);

            db.SaveChanges();
        }

    }
}