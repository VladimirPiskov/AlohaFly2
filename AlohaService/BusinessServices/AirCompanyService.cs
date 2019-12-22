using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;

namespace AlohaService.BusinessServices
{
    public class AirCompanyService
    {
        private AlohaDb db;

        public AirCompanyService(AlohaDb databaseContext)
        {
            db = databaseContext;
        }

        private static Dictionary<long, List<string>> flightCodesCash;

        private void FillFlightCodesCash()
        {
            flightCodesCash = new Dictionary<long, List<string>>();

            var allCodes = db.OrderFlight.Select(of => new { of.AirCompanyId, of.FlightNumber }).Distinct();

            foreach (var code in allCodes)
            {
                if (code.AirCompanyId != null && !flightCodesCash.ContainsKey(code.AirCompanyId.Value))
                {
                    flightCodesCash.Add(code.AirCompanyId.Value, new List<string>());
                }

                if (!flightCodesCash[code.AirCompanyId.Value].Contains(code.FlightNumber))
                {
                    flightCodesCash[code.AirCompanyId.Value].Add(code.FlightNumber);
                }
            }
        }

        public OperationResultValue<List<string>> GetFlightCodes(long airCompanyId, string startsWith)
        {
            if (flightCodesCash == null)
            {
                FillFlightCodesCash();
            }

            if (string.IsNullOrEmpty(startsWith))
            {
                if (flightCodesCash.ContainsKey(airCompanyId))
                {
                    return new OperationResultValue<List<string>>
                    {
                        Result = flightCodesCash[airCompanyId].ToList(),
                        Success = true
                    };
                }
                else
                {
                    return new OperationResultValue<List<string>>
                    {
                        Result = new List<string>(),
                        Success = true
                    };
                }
            }
            else
            {
                if (flightCodesCash.ContainsKey(airCompanyId))
                {
                    return new OperationResultValue<List<string>>
                    {
                        Result = flightCodesCash[airCompanyId].Where(t => t.StartsWith(startsWith)).ToList(),
                        Success = true
                    };
                }
                else
                {
                    return new OperationResultValue<List<string>>
                    {
                        Result = new List<string>(),
                        Success = true
                    };
                }
            }
        }

        public long CreateAirCompany(ServiceDataContracts.AirCompany airCompany)
        {
            var ac = new Entities.AirCompany();
            ac.Code = airCompany.Code;
            ac.PaymentId = airCompany.PaymentId;
            ac.DiscountId = airCompany.DiscountId;
            ac.IsActive = airCompany.IsActive;
            ac.SHId = airCompany.SHId;
            ac.Code1C = airCompany.Code1C;
            ac.Name1C = airCompany.Name1C;
            ac.Name = airCompany.Name;
            ac.Address = airCompany.Address;
            ac.FullName = airCompany.FullName;
            ac.IataCode = airCompany.IataCode;
            ac.Inn = airCompany.Inn;
            ac.RussianCode = airCompany.RussianCode;
            ac.IkaoCode = airCompany.IkaoCode;

            db.AirCompanies.Add(ac);
            db.SaveChanges();

            return ac.Id;
        }

        public ServiceDataContracts.AirCompany GetAirCompany(long airCompanyId)
        {
            var airCompany = db.AirCompanies.FirstOrDefault(ac => ac.Id == airCompanyId);
            var result = new ServiceDataContracts.AirCompany();

            result.Code = airCompany.Code;
            result.PaymentId = airCompany.PaymentId;
            result.PaymentType = airCompany.PaymentType == null ? null :
                new ServiceDataContracts.Payment
                {
                    Code = airCompany.PaymentType.Code,
                    Id = airCompany.PaymentType.Id,
                    FiskalId = airCompany.PaymentType.FiskalId,
                    IsCash = airCompany.PaymentType.IsCash,
                    Name = airCompany.PaymentType.Name
                };
            result.DiscountType = airCompany.DiscountType == null ? null :
                new ServiceDataContracts.Discount
                {
                    Id = airCompany.DiscountType.Id,
                    Name = airCompany.DiscountType.Name,
                    //Ranges = airCompany.DiscountType.Ranges.Select(r => new ServiceDataContracts.DiscountRange
                    //{
                    //    DiscountPercent = r.DiscountPercent,
                    //    End = r.End,
                    //    Id = r.Id,
                    //    Start = r.Start
                    //}).ToList()
                };
            result.DiscountId = airCompany.DiscountId;
            result.Id = airCompany.Id;
            result.SHId = airCompany.SHId;
            result.Name1C = airCompany.Name1C;
            result.Code1C = airCompany.Code1C;
            result.IsActive = airCompany.IsActive;
            result.Name = airCompany.Name;
            result.Address = airCompany.Address;
            result.FullName = airCompany.FullName;
            result.IataCode = airCompany.IataCode;
            result.Inn = airCompany.Inn;
            result.RussianCode = airCompany.RussianCode;
            result.IkaoCode = airCompany.IkaoCode;

            return result;
        }

        public void UpdateAirCompany(ServiceDataContracts.AirCompany airCompany)
        {
            var airCompanyToUpdate = db.AirCompanies.FirstOrDefault(ac => ac.Id == airCompany.Id);
            airCompanyToUpdate.Code = airCompany.Code;
            airCompanyToUpdate.PaymentId = airCompany.PaymentId;
            airCompanyToUpdate.DiscountId = airCompany.DiscountId;
            airCompanyToUpdate.IsActive = airCompany.IsActive;
            airCompanyToUpdate.SHId = airCompany.SHId;
            airCompanyToUpdate.Code1C = airCompany.Code1C;
            airCompanyToUpdate.Name1C = airCompany.Name1C;
            airCompanyToUpdate.Name = airCompany.Name;
            airCompanyToUpdate.Address = airCompany.Address;
            airCompanyToUpdate.FullName = airCompany.FullName;
            airCompanyToUpdate.IataCode = airCompany.IataCode;
            airCompanyToUpdate.Inn = airCompany.Inn;
            airCompanyToUpdate.RussianCode = airCompany.RussianCode;
            airCompanyToUpdate.IkaoCode = airCompany.IkaoCode;

            db.SaveChanges();
        }

        public void DeleteAirCompany(long airCompanyId)
        {
            var airCompany = db.AirCompanies.FirstOrDefault(ac => ac.Id == airCompanyId);
            db.AirCompanies.Remove(airCompany);
            db.SaveChanges();
        }

        public List<ServiceDataContracts.AirCompany> GetAirCompanyList()
        {
            return db.AirCompanies.ToList().Select(
                ac => new ServiceDataContracts.AirCompany
                {
                    Id = ac.Id,
                    Code = ac.Code,
                    PaymentId = ac.PaymentId,
                    PaymentType = ac.PaymentType == null ? null :
                    new ServiceDataContracts.Payment
                    {
                        Code = ac.PaymentType.Code,
                        Id = ac.PaymentType.Id,
                        FiskalId = ac.PaymentType.FiskalId,
                        IsCash = ac.PaymentType.IsCash,
                        Name = ac.PaymentType.Name
                    },
                    DiscountId = ac.DiscountId,
                    DiscountType = ac.DiscountType == null ? null :
                    new ServiceDataContracts.Discount
                    {
                        Id = ac.DiscountType.Id,
                        Name = ac.DiscountType.Name,
                        //Ranges = ac.DiscountType.Ranges.Select(r => new ServiceDataContracts.DiscountRange
                        //{
                        //    DiscountPercent = r.DiscountPercent,
                        //    End = r.End,
                        //    Id = r.Id,
                        //    Start = r.Start
                        //}).ToList()
                    },
                    IsActive = ac.IsActive,
                    SHId = ac.SHId,
                    Name = ac.Name,
                    Code1C = ac.Code1C,
                    Name1C = ac.Name1C
                }).ToList();
        }
    }
}