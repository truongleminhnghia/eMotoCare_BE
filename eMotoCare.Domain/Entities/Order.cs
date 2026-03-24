using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid PackageId { get; private set; }
        public virtual Package Package { get; set; } = null!;

        public Guid CustomerId { get; private set; }
        public virtual Customer Customer { get; set; } = null!;

        public int Duration { get; private set; }
        public PackageUnit Unit { get; private set; }
        public decimal TotalAmount { get; private set; }
        public OrderStatus Status { get; private set; }

        private Order()
        {
            // For EF Core
        }

        public Order(
            Guid id,
            Guid packageId,
            Guid customerId,
            int duration,
            PackageUnit unit,
            decimal totalAmount)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            PackageId = ValidateGuid(packageId, nameof(packageId));
            CustomerId = ValidateGuid(customerId, nameof(customerId));

            Duration = ValidateDuration(duration);
            Unit = unit;
            TotalAmount = ValidateMoney(totalAmount, nameof(totalAmount));

            Status = OrderStatus.PENDING;
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

        public void ChangeTotalAmount(decimal totalAmount)
        {
            TotalAmount = ValidateMoney(totalAmount, nameof(totalAmount));
            Touch();
        }

        public void ChangePackage(Guid packageId)
        {
            PackageId = ValidateGuid(packageId, nameof(packageId));
            Touch();
        }

        public void ChangeCustomer(Guid customerId)
        {
            CustomerId = ValidateGuid(customerId, nameof(customerId));
            Touch();
        }

        public void Confirm()
        {
            if (Status != OrderStatus.PENDING)
                throw new InvalidOperationException("Only PENDING orders can be confirmed.");

            Status = OrderStatus.CONFIRMED;
            Touch();
        }

        public void MarkProcessing()
        {
            if (Status != OrderStatus.CONFIRMED)
                throw new InvalidOperationException("Only CONFIRMED orders can be marked as PROCESSING.");

            Status = OrderStatus.PROCESSING;
            Touch();
        }

        public void MarkCompleted()
        {
            if (Status != OrderStatus.PROCESSING)
                throw new InvalidOperationException("Only PROCESSING orders can be completed.");

            Status = OrderStatus.COMPLETED;
            Touch();
        }

        public void Cancel()
        {
            if (Status == OrderStatus.COMPLETED || Status == OrderStatus.CANCELLED)
                throw new InvalidOperationException("Cannot cancel completed or already cancelled orders.");

            Status = OrderStatus.CANCELLED;
            Touch();
        }

        public void SetStatus(OrderStatus status)
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

        private static decimal ValidateMoney(decimal value, string fieldName)
        {
            if (value < 0m)
                throw new ArgumentOutOfRangeException(fieldName, $"{fieldName} cannot be negative.");

            return Math.Round(value, 2);
        }
    }
}
