using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class CustomerPackage : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid PackageId { get; private set; }
        public virtual Package Package { get; set; } = null!;

        public Guid CustomerId { get; private set; }
        public virtual Customer Customer { get; set; } = null!;

        public int Duration { get; private set; }
        public PackageUnit Unit { get; private set; }

        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }

        public CustomerPackageStatus Status { get; private set; }
        public DateTime? ExpiryTime { get; private set; }

        private CustomerPackage()
        {
            // For EF Core
        }

        public CustomerPackage(
            Guid id,
            Guid packageId,
            Guid customerId,
            int duration,
            PackageUnit unit,
            DateOnly startDate,
            DateOnly endDate,
            DateTime? expiryTime = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            PackageId = ValidateGuid(packageId, nameof(packageId));
            CustomerId = ValidateGuid(customerId, nameof(customerId));

            Duration = ValidateDuration(duration);
            Unit = unit;

            ValidateDateRange(startDate, endDate);
            StartDate = startDate;
            EndDate = endDate;

            ExpiryTime = expiryTime;
            Status = CustomerPackageStatus.ACTIVE;
        }

        public void ChangeDuration(int duration)
        {
            Duration = ValidateDuration(duration);
            Touch();
        }

        public void ChangeUnit(PackageUnit unit)
        {
            Unit = unit;
            Touch();
        }

        public void ChangeDateRange(DateOnly startDate, DateOnly endDate)
        {
            ValidateDateRange(startDate, endDate);
            StartDate = startDate;
            EndDate = endDate;
            Touch();
        }

        public void ChangeExpiryTime(DateTime? expiryTime)
        {
            ExpiryTime = expiryTime;
            Touch();
        }

        public void Activate()
        {
            Status = CustomerPackageStatus.ACTIVE;
            Touch();
        }

        public void Expire()
        {
            if (Status == CustomerPackageStatus.CANCELLED)
                throw new InvalidOperationException("Cannot expire a cancelled package.");

            Status = CustomerPackageStatus.EXPIRED;
            ExpiryTime = DateTime.UtcNow;
            Touch();
        }

        public void Cancel()
        {
            if (Status == CustomerPackageStatus.CANCELLED || Status == CustomerPackageStatus.EXPIRED)
                throw new InvalidOperationException("Cannot cancel an expired or already cancelled package.");

            Status = CustomerPackageStatus.CANCELLED;
            Touch();
        }

        public void Suspend()
        {
            if (Status != CustomerPackageStatus.ACTIVE)
                throw new InvalidOperationException("Only ACTIVE packages can be suspended.");

            Status = CustomerPackageStatus.SUSPENDED;
            Touch();
        }

        public void Resume()
        {
            if (Status != CustomerPackageStatus.SUSPENDED)
                throw new InvalidOperationException("Only SUSPENDED packages can be resumed.");

            Status = CustomerPackageStatus.ACTIVE;
            Touch();
        }

        public bool IsExpired(DateOnly currentDate)
            => currentDate > EndDate;

        public bool IsActive(DateOnly currentDate)
            => Status == CustomerPackageStatus.ACTIVE && currentDate >= StartDate && currentDate <= EndDate;

        public void SetStatus(CustomerPackageStatus status)
        {
            Status = status;
            Touch();
        }

        private static Guid ValidateGuid(Guid value, string fieldName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value;
        }

        private static int ValidateDuration(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Duration must be greater than 0.");

            return value;
        }

        private static void ValidateDateRange(DateOnly startDate, DateOnly endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("EndDate must be greater than or equal to StartDate.");
        }
    }
}
