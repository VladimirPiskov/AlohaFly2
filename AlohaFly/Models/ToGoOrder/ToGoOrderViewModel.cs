using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaFly.Models.ToGoOrder
{
   public  class OrderToGoViewModel
    {
        private OrderToGo order;
        public OrderToGoViewModel(OrderToGo _order)
        {
            order = _order;
        }

        //public User CreatedBy { get { return DataExtension.DataCatalogsSingleton.Instance.} }

    }
}
