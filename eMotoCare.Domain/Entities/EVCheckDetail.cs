using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class EVCheckDetail : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid EVCheckId { get; private set; }
        public virtual EVCheck EVCheck { get; set; } = null!;

        public Guid? ReplacementPartItemId { get; private set; }
        public virtual PartItem? ReplacementPartItem { get; set; }

        public Guid StageDetailId { get; private set; }
        public virtual MaintenanceStageDetail StageDetail { get; set; } = null!;

        public string Result { get; private set; } = null!;
        public string? ImageResult { get; private set; }
        public string? Remedies { get; private set; }

        public PartUnit Unit { get; private set; }
        public int Quantity { get; private set; }

        public decimal PricePart { get; private set; }
        public decimal PriceService { get; private set; }
        public decimal TotalAmount { get; private set; } // Calculated: (PricePart + PriceService) * Quantity

        public EVCheckDetailStatus Status { get; private set; }

        private EVCheckDetail()
        {
            // For EF Core
        }

        public EVCheckDetail(
            Guid id,
            Guid evCheckId,
            Guid stageDetailId,
            string result,
            PartUnit unit,
            int quantity,
            decimal pricePart,
            decimal priceService,
            Guid? replacementPartItemId = null,
            string? imageResult = null,
            string? remedies = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            EVCheckId = ValidateGuid(evCheckId, nameof(evCheckId));
            StageDetailId = ValidateGuid(stageDetailId, nameof(stageDetailId));
            ReplacementPartItemId = ValidateNullableGuid(replacementPartItemId, nameof(replacementPartItemId));

            Result = ValidateRequired(result, nameof(result));
            ImageResult = NormalizeNullable(imageResult);
            Remedies = NormalizeNullable(remedies);

            Unit = unit;
            Quantity = ValidateQuantity(quantity);
            PricePart = ValidateMoney(pricePart, nameof(pricePart));
            PriceService = ValidateMoney(priceService, nameof(priceService));
            TotalAmount = CalculateTotalAmount(Quantity, PricePart, PriceService);

            Status = EVCheckDetailStatus.PENDING;
        }

        public void ChangeResult(string result)
        {
            Result = ValidateRequired(result, nameof(result));
            Touch();
        }

        public void ChangeImageResult(string? imageResult)
        {
            ImageResult = NormalizeNullable(imageResult);
            Touch();
        }

        public void ChangeRemedies(string? remedies)
        {
            Remedies = NormalizeNullable(remedies);
            Touch();
        }

        public void ChangePrices(decimal pricePart, decimal priceService)
        {
            PricePart = ValidateMoney(pricePart, nameof(pricePart));
            PriceService = ValidateMoney(priceService, nameof(priceService));
            TotalAmount = CalculateTotalAmount(Quantity, PricePart, PriceService);
            Touch();
        }

        public void ChangeQuantity(int quantity)
        {
            Quantity = ValidateQuantity(quantity);
            TotalAmount = CalculateTotalAmount(Quantity, PricePart, PriceService);
            Touch();
        }

        public void ChangeUnit(PartUnit unit)
        {
            Unit = unit;
            Touch();
        }

        public void ChangeReplacementPartItem(Guid? replacementPartItemId)
        {
            ReplacementPartItemId = ValidateNullableGuid(replacementPartItemId, nameof(replacementPartItemId));
            Touch();
        }

        public void MarkInspected()
        {
            if (Status != EVCheckDetailStatus.PENDING)
                throw new InvalidOperationException("Only PENDING details can be marked as INSPECTED.");

            Status = EVCheckDetailStatus.INSPECTED;
            Touch();
        }

        public void MarkPassed()
        {
            if (Status != EVCheckDetailStatus.INSPECTED)
                throw new InvalidOperationException("Only INSPECTED details can be marked as PASSED.");

            Status = EVCheckDetailStatus.PASSED;
            Touch();
        }

        public void MarkFailed(string? remedies = null)
        {
            if (Status != EVCheckDetailStatus.INSPECTED)
                throw new InvalidOperationException("Only INSPECTED details can be marked as FAILED.");

            if (!string.IsNullOrWhiteSpace(remedies))
                Remedies = NormalizeNullable(remedies) ?? Remedies;

            Status = EVCheckDetailStatus.FAILED;
            Touch();
        }

        public void MarkRemedied()
        {
            if (Status != EVCheckDetailStatus.FAILED)
                throw new InvalidOperationException("Only FAILED details can be marked as REMEDIED.");

            Status = EVCheckDetailStatus.REMEDIED;
            Touch();
        }

        public void SetStatus(EVCheckDetailStatus status)
        {
            Status = status;
            Touch();
        }

        private static decimal CalculateTotalAmount(int quantity, decimal pricePart, decimal priceService)
            => Math.Round((pricePart + priceService) * quantity, 2);

        private static Guid ValidateGuid(Guid value, string fieldName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value;
        }

        private static Guid? ValidateNullableGuid(Guid? value, string fieldName)
        {
            if (value.HasValue && value.Value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty Guid.", fieldName);

            return value;
        }

        private static int ValidateQuantity(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Quantity must be greater than 0.");

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
