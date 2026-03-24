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
    public class Payment : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid? AppointmentId { get; private set; }
        public virtual Appointment? Appointment { get; set; }

        public Guid? OrderId { get; private set; }
        public virtual Order? Order { get; set; }

        public decimal TotalAmount { get; private set; }
        public decimal DepositAmount { get; private set; }
        public decimal RemainingAmount { get; private set; } // Calculated: TotalAmount - DepositAmount

        public DateTime PaymentDate { get; private set; }
        public Currency Currency { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }

        public string Code { get; private set; } = null!;
        public PaymentStatus Status { get; private set; }

        private Payment()
        {
            // For EF Core
        }

        public Payment(
            Guid id,
            decimal totalAmount,
            decimal depositAmount,
            DateTime paymentDate,
            Currency currency,
            PaymentMethod paymentMethod,
            string code,
            Guid? appointmentId = null,
            Guid? orderId = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;

            ValidateAtLeastOneFK(appointmentId, orderId);
            AppointmentId = ValidateNullableGuid(appointmentId, nameof(appointmentId));
            OrderId = ValidateNullableGuid(orderId, nameof(orderId));

            TotalAmount = ValidateMoney(totalAmount, nameof(totalAmount));
            DepositAmount = ValidateMoney(depositAmount, nameof(depositAmount));
            RemainingAmount = CalculateRemainingAmount(TotalAmount, DepositAmount);

            PaymentDate = paymentDate;
            Currency = currency;
            PaymentMethod = paymentMethod;
            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();

            Status = DetermineInitialStatus(DepositAmount, TotalAmount);
        }

        public static Payment CreateWithGeneratedCode(
            Guid id,
            string codeSeed,
            Func<string, bool> existsCodeInPaymentTable,
            decimal totalAmount,
            decimal depositAmount,
            DateTime paymentDate,
            Currency currency,
            PaymentMethod paymentMethod,
            Guid? appointmentId = null,
            Guid? orderId = null)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInPaymentTable);

            return new Payment(
                id,
                totalAmount,
                depositAmount,
                paymentDate,
                currency,
                paymentMethod,
                generatedCode,
                appointmentId,
                orderId);
        }

        public void ChangeDepositAmount(decimal depositAmount)
        {
            DepositAmount = ValidateMoney(depositAmount, nameof(depositAmount));
            RemainingAmount = CalculateRemainingAmount(TotalAmount, DepositAmount);
            Status = DetermineInitialStatus(DepositAmount, TotalAmount);
            Touch();
        }

        public void ChangeTotalAmount(decimal totalAmount)
        {
            TotalAmount = ValidateMoney(totalAmount, nameof(totalAmount));
            RemainingAmount = CalculateRemainingAmount(TotalAmount, DepositAmount);
            Touch();
        }

        public void ChangePaymentDate(DateTime paymentDate)
        {
            PaymentDate = paymentDate;
            Touch();
        }

        public void ChangePaymentMethod(PaymentMethod paymentMethod)
        {
            PaymentMethod = paymentMethod;
            Touch();
        }

        public void ChangeCurrency(Currency currency)
        {
            Currency = currency;
            Touch();
        }

        public void ProcessPayment(decimal amountPaid)
        {
            var validatedAmount = ValidateMoney(amountPaid, nameof(amountPaid));

            if (validatedAmount > RemainingAmount)
                throw new InvalidOperationException($"Payment amount {amountPaid} exceeds remaining amount {RemainingAmount}.");

            DepositAmount += validatedAmount;
            RemainingAmount = CalculateRemainingAmount(TotalAmount, DepositAmount);

            if (RemainingAmount == 0m)
                Status = PaymentStatus.COMPLETED;
            else if (DepositAmount > 0m)
                Status = PaymentStatus.PARTIAL;

            Touch();
        }

        public void MarkCompleted()
        {
            if (RemainingAmount > 0m)
                throw new InvalidOperationException("Cannot mark as completed while there is remaining amount.");

            Status = PaymentStatus.COMPLETED;
            Touch();
        }

        public void MarkOverdue()
        {
            if (Status == PaymentStatus.COMPLETED || Status == PaymentStatus.CANCELLED)
                throw new InvalidOperationException("Cannot mark completed or cancelled payments as overdue.");

            Status = PaymentStatus.OVERDUE;
            Touch();
        }

        public void Cancel()
        {
            if (Status == PaymentStatus.COMPLETED)
                throw new InvalidOperationException("Cannot cancel a completed payment.");

            Status = PaymentStatus.CANCELLED;
            Touch();
        }

        public void SetStatus(PaymentStatus status)
        {
            Status = status;
            Touch();
        }

        private static decimal CalculateRemainingAmount(decimal totalAmount, decimal depositAmount)
            => Math.Round(totalAmount - depositAmount, 2);

        private static PaymentStatus DetermineInitialStatus(decimal depositAmount, decimal totalAmount)
        {
            if (depositAmount >= totalAmount)
                return PaymentStatus.COMPLETED;

            return depositAmount > 0m ? PaymentStatus.PARTIAL : PaymentStatus.PENDING;
        }

        private static void ValidateAtLeastOneFK(Guid? appointmentId, Guid? orderId)
        {
            if (!appointmentId.HasValue && !orderId.HasValue)
                throw new ArgumentException("At least one of AppointmentId or OrderId must be provided.");
        }

        private static Guid? ValidateNullableGuid(Guid? value, string fieldName)
        {
            if (value.HasValue && value.Value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty Guid.", fieldName);

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
    }
}
