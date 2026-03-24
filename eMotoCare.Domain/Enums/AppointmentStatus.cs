using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Enums
{
    public enum AppointmentStatus
    {
        PENDING,
        CONFIRMED,
        CHECKED_IN,
        IN_PROGRESS,
        COMPLETED,
        CANCELLED,
        NO_SHOW
    }
}
