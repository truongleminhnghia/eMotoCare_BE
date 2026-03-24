using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class VehicleStage : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid VehicleId { get; private set; }
        public virtual Vehicle Vehicle { get; set; } = null!;

        public Guid StageId { get; private set; }
        public virtual MaintenanceStage Stage { get; set; } = null!;

        public VehicleStageStatus Status { get; private set; }
        public int ScheduledMileage { get; private set; }
        public int? ActualMileage { get; private set; }
        public string? Note { get; private set; }

        private VehicleStage()
        {
            // For EF Core
        }

        public VehicleStage(
            Guid id,
            Guid vehicleId,
            Guid stageId,
            int scheduledMileage,
            string? note = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            VehicleId = ValidateGuid(vehicleId, nameof(vehicleId));
            StageId = ValidateGuid(stageId, nameof(stageId));

            ScheduledMileage = ValidateMileage(scheduledMileage);
            Note = NormalizeNullable(note);

            Status = VehicleStageStatus.PENDING;
        }

        public void ChangeScheduledMileage(int scheduledMileage)
        {
            ScheduledMileage = ValidateMileage(scheduledMileage);
            Touch();
        }

        public void MarkInProgress()
        {
            if (Status != VehicleStageStatus.PENDING)
                throw new InvalidOperationException("Only PENDING stages can be marked as IN_PROGRESS.");

            Status = VehicleStageStatus.IN_PROGRESS;
            Touch();
        }

        public void MarkCompleted(int actualMileage, string? note = null)
        {
            if (Status != VehicleStageStatus.IN_PROGRESS)
                throw new InvalidOperationException("Only IN_PROGRESS stages can be completed.");

            ActualMileage = ValidateMileage(actualMileage);
            Note = NormalizeNullable(note) ?? Note;
            Status = VehicleStageStatus.COMPLETED;
            Touch();
        }

        public void MarkSkipped(string? reason = null)
        {
            if (Status == VehicleStageStatus.COMPLETED || Status == VehicleStageStatus.SKIPPED)
                throw new InvalidOperationException("Cannot skip completed or already skipped stages.");

            Note = NormalizeNullable(reason) ?? Note;
            Status = VehicleStageStatus.SKIPPED;
            Touch();
        }

        public void MarkOverdue()
        {
            if (Status == VehicleStageStatus.COMPLETED || Status == VehicleStageStatus.SKIPPED)
                throw new InvalidOperationException("Cannot mark completed or skipped stages as overdue.");

            Status = VehicleStageStatus.OVERDUE;
            Touch();
        }

        public void ChangeNote(string? note)
        {
            Note = NormalizeNullable(note);
            Touch();
        }

        public void ChangeVehicle(Guid vehicleId)
        {
            VehicleId = ValidateGuid(vehicleId, nameof(vehicleId));
            Touch();
        }

        public void ChangeStage(Guid stageId)
        {
            StageId = ValidateGuid(stageId, nameof(stageId));
            Touch();
        }

        public void SetStatus(VehicleStageStatus status)
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

        private static int ValidateMileage(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Mileage cannot be negative.");

            return value;
        }

        private static string? NormalizeNullable(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }
    }
}
