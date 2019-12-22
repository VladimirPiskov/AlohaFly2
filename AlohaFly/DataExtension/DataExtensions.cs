using AlohaService.ServiceDataContracts;
using System.Linq;

namespace AlohaFly.DataExtension
{
    public static class DataExtensions
    {
        public static bool PhoneEquals(this OrderCustomerPhone orig, OrderCustomerPhone phone)
        {
            if (phone == null) return false;
            return (orig.IsActive == phone.IsActive)
                && (orig.IsPrimary == phone.IsPrimary)
                && (orig.Phone == phone.Phone);
        }

        public static bool AddressEquals(this OrderCustomerAddress orig, OrderCustomerAddress phone)
        {
            if (phone == null) return false;
            return (orig.IsActive == phone.IsActive)
                && (orig.IsPrimary == phone.IsPrimary)
                && (orig.MapUrl == phone.MapUrl)
                && (orig.SubWay == phone.SubWay)
                && (orig.Address == phone.Address)
                && (orig.ZoneId == phone.ZoneId);
        }

        public static OrderCustomerPhone GetPrimaryPhone(this OrderCustomer orderCustomer)
        {
            if (orderCustomer == null) return null;
            if (DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data.Any(a => a.OrderCustomerId == orderCustomer.Id && a.IsPrimary))
            {
                return DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data.FirstOrDefault(a => a.OrderCustomerId == orderCustomer.Id && a.IsPrimary);
            }
            return null;
        }

        public static OrderCustomerAddress GetPrimaryAddress(this OrderCustomer orderCustomer)
        {
            if (orderCustomer == null) return null;
            if (DataCatalogsSingleton.Instance.OrderCustomerAddressData.Data.Any(a => a.OrderCustomerId == orderCustomer.Id && a.IsPrimary))
            {
                return DataCatalogsSingleton.Instance.OrderCustomerAddressData.Data.FirstOrDefault(a => a.OrderCustomerId == orderCustomer.Id && a.IsPrimary);
            }
            return null;
        }
    }
}
