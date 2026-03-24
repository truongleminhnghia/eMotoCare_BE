using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Enums
{
    public enum PaymentStatus
    {
        PENDING,
        PARTIAL,
        COMPLETED,
        OVERDUE,
        CANCELLED
    }
}
