using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class EVCheck : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid AppointmentId { get; private set; }
        public virtual Appointment Appointment { get; set; } = null!;

        public Guid Executor { get; private set; }
        public virtual Staff ExecutorStaff { get; set; } = null!;

        public DateTime CheckDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public EVCheckStatus Status { get; private set; }

        private EVCheck()
        {
            // For EF Core
        }

        public EVCheck(
            Guid id,
            Guid appointmentId,
            Guid executor,
            DateTime checkDate,
            decimal totalAmount)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            AppointmentId = ValidateGuid(appointmentId, nameof(appointmentId));
            Executor = ValidateGuid(executor, nameof(executor));

            CheckDate = checkDate;
            TotalAmount = ValidateMoney(totalAmount, nameof(totalAmount));

            Status = EVCheckStatus.PENDING;
        }

        public void ChangeCheckDate(DateTime checkDate)
        {
            CheckDate = checkDate;
            Touch();
        }

        public void ChangeTotalAmount(decimal totalAmount)
        {
            TotalAmount = ValidateMoney(totalAmount, nameof(totalAmount));
            Touch();
        }

        public void ChangeExecutor(Guid executor)
        {
            Executor = ValidateGuid(executor, nameof(executor));
            Touch();
        }

        public void MarkInProgress()
        {
            if (Status != EVCheckStatus.PENDING)
                throw new InvalidOperationException("Only PENDING checks can be marked as IN_PROGRESS.");

            Status = EVCheckStatus.IN_PROGRESS;
            Touch();
        }

        public void MarkCompleted()
        {
            if (Status != EVCheckStatus.IN_PROGRESS)
                throw new InvalidOperationException("Only IN_PROGRESS checks can be completed.");

            Status = EVCheckStatus.COMPLETED;
            Touch();
        }

        public void MarkPassed()
        {
            if (Status != EVCheckStatus.COMPLETED)
                throw new InvalidOperationException("Only COMPLETED checks can be marked as PASSED.");

            Status = EVCheckStatus.PASSED;
            Touch();
        }

        public void MarkFailed()
        {
            if (Status != EVCheckStatus.COMPLETED)
                throw new InvalidOperationException("Only COMPLETED checks can be marked as FAILED.");

            Status = EVCheckStatus.FAILED;
            Touch();
        }

        public void SetStatus(EVCheckStatus status)
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
    }
}
