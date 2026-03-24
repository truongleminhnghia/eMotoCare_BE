using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class RMA : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid CreateBy { get; private set; }
        public virtual Staff CreatedByStaff { get; set; } = null!;

        public Guid? ApproveBy { get; private set; }
        public virtual Staff? ApprovedByStaff { get; set; }

        public RMAStatus Status { get; private set; }
        public string Reason { get; private set; } = null!;

        public Guid CustomerId { get; private set; }
        public virtual Customer Customer { get; set; } = null!;

        public Guid VehicleId { get; private set; }
        public virtual Vehicle Vehicle { get; set; } = null!;

        public string? Note { get; private set; }

        public DateTime? ProcessedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        private RMA()
        {
            // For EF Core
        }

        public RMA(
            Guid id,
            Guid createBy,
            Guid customerId,
            Guid vehicleId,
            string reason,
            string? note = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            CreateBy = ValidateGuid(createBy, nameof(createBy));
            CustomerId = ValidateGuid(customerId, nameof(customerId));
            VehicleId = ValidateGuid(vehicleId, nameof(vehicleId));

            Reason = ValidateRequired(reason, nameof(reason));
            Note = NormalizeNullable(note);

            Status = RMAStatus.PENDING;
        }

        public void ChangeReason(string reason)
        {
            Reason = ValidateRequired(reason, nameof(reason));
            Touch();
        }

        public void ChangeNote(string? note)
        {
            Note = NormalizeNullable(note);
            Touch();
        }

        public void ChangeCustomer(Guid customerId)
        {
            CustomerId = ValidateGuid(customerId, nameof(customerId));
            Touch();
        }

        public void ChangeVehicle(Guid vehicleId)
        {
            VehicleId = ValidateGuid(vehicleId, nameof(vehicleId));
            Touch();
        }

        public void Approve(Guid approveBy, DateTime? processedAtUtc = null)
        {
            if (Status != RMAStatus.PENDING)
                throw new InvalidOperationException("Only PENDING RMA can be approved.");

            ApproveBy = ValidateGuid(approveBy, nameof(approveBy));
            ProcessedAt = processedAtUtc ?? DateTime.UtcNow;
            Status = RMAStatus.APPROVED;
            Touch();
        }

        public void Reject(Guid approveBy, string? reason = null, DateTime? processedAtUtc = null)
        {
            if (Status != RMAStatus.PENDING)
                throw new InvalidOperationException("Only PENDING RMA can be rejected.");

            ApproveBy = ValidateGuid(approveBy, nameof(approveBy));
            ProcessedAt = processedAtUtc ?? DateTime.UtcNow;
            Status = RMAStatus.REJECTED;

            if (!string.IsNullOrWhiteSpace(reason))
                Note = NormalizeNullable(reason) ?? Note;

            Touch();
        }

        public void StartProcessing()
        {
            if (Status != RMAStatus.APPROVED)
                throw new InvalidOperationException("Only APPROVED RMA can start processing.");

            Status = RMAStatus.PROCESSING;
            Touch();
        }

        public void Complete(DateTime? completedAtUtc = null)
        {
            if (Status != RMAStatus.PROCESSING)
                throw new InvalidOperationException("Only PROCESSING RMA can be completed.");

            CompletedAt = completedAtUtc ?? DateTime.UtcNow;
            Status = RMAStatus.COMPLETED;
            Touch();
        }

        public void Cancel()
        {
            if (Status == RMAStatus.COMPLETED || Status == RMAStatus.CANCELLED)
                throw new InvalidOperationException("Cannot cancel completed or already cancelled RMA.");

            Status = RMAStatus.CANCELLED;
            Touch();
        }

        public void SetStatus(RMAStatus status)
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

        private static string ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value.Trim();
        }

        private static string? NormalizeNullable(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }
    }
}
