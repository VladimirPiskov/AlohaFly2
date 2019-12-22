using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaService.Interfaces
{
    public  interface IPrimaryUnik
    {
        bool IsPrimary { get; set; }
        long OrderCustomerId { get; set; }
    }

    public interface IFocusable
    {
        bool IsFocused { get; set; }
    }
}
