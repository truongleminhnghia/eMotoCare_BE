using eMotoCare.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class PackageCondition : BaseEntity
    {
        public Guid Id { get; private set; }

        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }

        private PackageCondition()
        {
            // For EF Core
        }

        public PackageCondition(
            Guid id,
            DateOnly startDate,
            DateOnly endDate)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            ValidateDateRange(startDate, endDate);

            StartDate = startDate;
            EndDate = endDate;
        }

        public void ChangeDateRange(DateOnly startDate, DateOnly endDate)
        {
            ValidateDateRange(startDate, endDate);
            StartDate = startDate;
            EndDate = endDate;
            Touch();
        }

        public bool IsActive(DateOnly currentDate)
            => currentDate >= StartDate && currentDate <= EndDate;

        private static void ValidateDateRange(DateOnly startDate, DateOnly endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("EndDate must be greater than or equal to StartDate.");
        }
    }
}
