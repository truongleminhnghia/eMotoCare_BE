using eMotoCare.Domain.Base;
using eMotoCare.Domain.Base.Common;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid? ApproveBy { get; private set; }
        public virtual Staff? ApprovedByStaff { get; set; }

        public Guid VehicleId { get; private set; }
        public virtual Vehicle Vehicle { get; set; } = null!;

        public Guid VehicleStageId { get; private set; }
        public virtual VehicleStage VehicleStage { get; set; } = null!;

        public Guid SlotTimeId { get; private set; }
        public virtual SlotTime SlotTime { get; set; } = null!;

        public Guid CustomerId { get; private set; }
        public virtual Customer Customer { get; set; } = null!;

        public string Reason { get; private set; } = null!;
        public AppointmentType AppointmentType { get; private set; }

        public string? QRCheckIn { get; private set; }
        public decimal TotalPrice { get; private set; }

        public string Code { get; private set; } = null!;
        public DateTime? TimeCheckIn { get; private set; }

        public AppointmentStatus Status { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }

        private Appointment()
        {
            // For EF Core
        }

        public Appointment(
            Guid id,
            Guid vehicleId,
            Guid vehicleStageId,
            Guid slotTimeId,
            Guid customerId,
            string code,
            string reason,
            AppointmentType appointmentType,
            decimal totalPrice,
            string? qrCheckIn = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            VehicleId = ValidateGuid(vehicleId, nameof(vehicleId));
            VehicleStageId = ValidateGuid(vehicleStageId, nameof(vehicleStageId));
            SlotTimeId = ValidateGuid(slotTimeId, nameof(slotTimeId));
            CustomerId = ValidateGuid(customerId, nameof(customerId));

            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();
            Reason = ValidateRequired(reason, nameof(reason));
            AppointmentType = appointmentType;
            TotalPrice = ValidateMoney(totalPrice, nameof(totalPrice));
            QRCheckIn = NormalizeNullable(qrCheckIn);

            Status = AppointmentStatus.PENDING;
        }

        public static Appointment CreateWithGeneratedCode(
            Guid id,
            Guid vehicleId,
            Guid vehicleStageId,
            Guid slotTimeId,
            Guid customerId,
            string codeSeed,
            Func<string, bool> existsCodeInAppointmentTable,
            string reason,
            AppointmentType appointmentType,
            decimal totalPrice,
            string? qrCheckIn = null)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInAppointmentTable);

            return new Appointment(
                id,
                vehicleId,
                vehicleStageId,
                slotTimeId,
                customerId,
                generatedCode,
                reason,
                appointmentType,
                totalPrice,
                qrCheckIn);
        }

        public void ChangeReason(string reason)
        {
            Reason = ValidateRequired(reason, nameof(reason));
            Touch();
        }

        public void ChangeAppointmentType(AppointmentType appointmentType)
        {
            AppointmentType = appointmentType;
            Touch();
        }

        public void ChangeTotalPrice(decimal totalPrice)
        {
            TotalPrice = ValidateMoney(totalPrice, nameof(totalPrice));
            Touch();
        }

        public void ChangeSlotTime(Guid slotTimeId)
        {
            SlotTimeId = ValidateGuid(slotTimeId, nameof(slotTimeId));
            Touch();
        }

        public void ChangeVehicleStage(Guid vehicleStageId)
        {
            VehicleStageId = ValidateGuid(vehicleStageId, nameof(vehicleStageId));
            Touch();
        }

        public void Confirm(Guid approveBy, DateTime? confirmedAtUtc = null)
        {
            if (Status != AppointmentStatus.PENDING)
                throw new InvalidOperationException("Only PENDING appointments can be confirmed.");

            ApproveBy = ValidateGuid(approveBy, nameof(approveBy));
            ConfirmedAt = confirmedAtUtc ?? DateTime.UtcNow;
            Status = AppointmentStatus.CONFIRMED;
            Touch();
        }

        public void CheckIn(string? qrCheckIn = null, DateTime? checkInTimeUtc = null)
        {
            if (Status != AppointmentStatus.CONFIRMED)
                throw new InvalidOperationException("Only CONFIRMED appointments can be checked in.");

            QRCheckIn = NormalizeNullable(qrCheckIn) ?? QRCheckIn;
            TimeCheckIn = checkInTimeUtc ?? DateTime.UtcNow;
            Status = AppointmentStatus.CHECKED_IN;
            Touch();
        }

        public void StartProgress()
        {
            if (Status != AppointmentStatus.CHECKED_IN)
                throw new InvalidOperationException("Only CHECKED_IN appointments can be marked as IN_PROGRESS.");

            Status = AppointmentStatus.IN_PROGRESS;
            Touch();
        }

        public void MarkCompleted()
        {
            if (Status != AppointmentStatus.IN_PROGRESS)
                throw new InvalidOperationException("Only IN_PROGRESS appointments can be completed.");

            Status = AppointmentStatus.COMPLETED;
            Touch();
        }

        public void MarkNoShow()
        {
            if (Status == AppointmentStatus.COMPLETED || Status == AppointmentStatus.CANCELLED)
                throw new InvalidOperationException("Cannot mark completed or cancelled appointments as NO_SHOW.");

            Status = AppointmentStatus.NO_SHOW;
            Touch();
        }

        public void Cancel()
        {
            if (Status == AppointmentStatus.COMPLETED || Status == AppointmentStatus.CANCELLED)
                throw new InvalidOperationException("Cannot cancel completed or already cancelled appointments.");

            Status = AppointmentStatus.CANCELLED;
            Touch();
        }

        public void SetStatus(AppointmentStatus status)
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

        private static decimal ValidateMoney(decimal value, string fieldName)
        {
            if (value < 0m)
                throw new ArgumentOutOfRangeException(fieldName, $"{fieldName} cannot be negative.");

            return Math.Round(value, 2);
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
