using AlohaService.ServiceDataContracts;
using System;

namespace AlohaFly.Import
{
    interface IOrderOrderFlightImport
    {
        OrderFlight GetOrder(Uri filePath);
    }
}
