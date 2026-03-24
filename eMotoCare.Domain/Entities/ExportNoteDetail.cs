using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;

namespace eMotoCare.Domain.Entities
{
    public class ExportNoteDetail : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid ExportNoteId { get; private set; }
        public virtual ExportNote ExportNote { get; set; } = null!;

        public Guid PartItemId { get; private set; }
        public virtual PartItem PartItem { get; set; } = null!;

        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice { get; private set; } // Calculated: Quantity * UnitPrice

        public string? Note { get; private set; }
        public StockStatus StockStatus { get; private set; }

        private ExportNoteDetail()
        {
            // For EF Core
        }

        public ExportNoteDetail(
            Guid id,
            Guid exportNoteId,
            Guid partItemId,
            int quantity,
            decimal unitPrice,
            StockStatus stockStatus,
            string? note = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            ExportNoteId = ValidateGuid(exportNoteId, nameof(exportNoteId));
            PartItemId = ValidateGuid(partItemId, nameof(partItemId));

            Quantity = ValidateQuantity(quantity);
            UnitPrice = ValidateMoney(unitPrice, nameof(unitPrice));
            TotalPrice = CalculateTotalPrice(Quantity, UnitPrice);

            StockStatus = stockStatus;
            Note = NormalizeNullable(note);
        }

        public void ChangeQuantity(int quantity)
        {
            Quantity = ValidateQuantity(quantity);
            TotalPrice = CalculateTotalPrice(Quantity, UnitPrice);
            Touch();
        }

        public void ChangeUnitPrice(decimal unitPrice)
        {
            UnitPrice = ValidateMoney(unitPrice, nameof(unitPrice));
            TotalPrice = CalculateTotalPrice(Quantity, UnitPrice);
            Touch();
        }

        public void ChangeNote(string? note)
        {
            Note = NormalizeNullable(note);
            Touch();
        }

        public void SetStockStatus(StockStatus stockStatus)
        {
            StockStatus = stockStatus;
            Touch();
        }

        public void ChangePartItem(Guid partItemId)
        {
            PartItemId = ValidateGuid(partItemId, nameof(partItemId));
            Touch();
        }

        public void ChangeExportNote(Guid exportNoteId)
        {
            ExportNoteId = ValidateGuid(exportNoteId, nameof(exportNoteId));
            Touch();
        }

        private static decimal CalculateTotalPrice(int quantity, decimal unitPrice)
            => Math.Round(quantity * unitPrice, 2);

        private static Guid ValidateGuid(Guid value, string fieldName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

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

        private static string? NormalizeNullable(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }
    }
}
