using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessing.Core.DTO.Orders
{
    public enum OrderStatus
    {
        Pending=0,
        Completed=1,
        Rejected=2,
    }
}
