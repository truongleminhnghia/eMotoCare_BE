using eMotoCare.Domain.Base;
using eMotoCare.Domain.Base.Common;
using eMotoCare.Domain.Enums;

namespace eMotoCare.Domain.Entities
{
    public class ImportNote : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid CreateBy { get; private set; }
        public virtual Staff CreatedByStaff { get; set; } = null!;

        public Guid InventoryId { get; private set; }
        public virtual Inventory Inventory { get; set; } = null!;

        public ImportType ImportType { get; private set; }

        public Guid? ApproveBy { get; private set; }
        public virtual Staff? ApprovedByStaff { get; set; }

        public string Code { get; private set; } = null!;
        public ImportStatus Status { get; private set; }

        public decimal TotalAmount { get; private set; }
        public string? Note { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }

        private ImportNote()
        {
            // For EF Core
        }

        public ImportNote(
            Guid id,
            Guid createBy,
            Guid inventoryId,
            string code,
            ImportType importType,
            decimal totalAmount,
            string? note = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            CreateBy = ValidateGuid(createBy, nameof(createBy));
            InventoryId = ValidateGuid(inventoryId, nameof(inventoryId));

            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();
            ImportType = importType;
            TotalAmount = ValidateMoney(totalAmount, nameof(totalAmount));
            Note = NormalizeNullable(note);

            Status = ImportStatus.PENDING;
        }

        public static ImportNote CreateWithGeneratedCode(
            Guid id,
            Guid createBy,
            Guid inventoryId,
            string codeSeed,
            Func<string, bool> existsCodeInImportNoteTable,
            ImportType importType,
            decimal totalAmount,
            string? note = null)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInImportNoteTable);

            return new ImportNote(
                id,
                createBy,
                inventoryId,
                generatedCode,
                importType,
                totalAmount,
                note);
        }

        public void ChangeImportType(ImportType importType)
        {
            ImportType = importType;
            Touch();
        }

        public void ChangeTotalAmount(decimal totalAmount)
        {
            TotalAmount = ValidateMoney(totalAmount, nameof(totalAmount));
            Touch();
        }

        public void ChangeNote(string? note)
        {
            Note = NormalizeNullable(note);
            Touch();
        }

        public void ChangeInventory(Guid inventoryId)
        {
            InventoryId = ValidateGuid(inventoryId, nameof(inventoryId));
            Touch();
        }

        public void Confirm(Guid approveBy, DateTime? confirmedAtUtc = null)
        {
            if (Status != ImportStatus.PENDING)
                throw new InvalidOperationException("Only PENDING ImportNote can be confirmed.");

            ApproveBy = ValidateGuid(approveBy, nameof(approveBy));
            ConfirmedAt = confirmedAtUtc ?? DateTime.UtcNow;
            Status = ImportStatus.CONFIRM;
            Touch();
        }

        public void Cancel()
        {
            if (Status == ImportStatus.CONFIRM)
                throw new InvalidOperationException("Cannot cancel confirmed ImportNote.");

            Status = ImportStatus.CANCELLED;
            Touch();
        }

        public void SetStatus(ImportStatus status)
        {
            Status = status;
            if (status == ImportStatus.CONFIRM && !ConfirmedAt.HasValue)
                ConfirmedAt = DateTime.UtcNow;

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
