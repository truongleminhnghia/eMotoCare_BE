using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
namespace eMotoCare.Domain.Entities
{
    public class ExportNote : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid CreateBy { get; private set; }
        public virtual Staff CreatedByStaff { get; set; } = null!;
        public Guid InventoryId { get; private set; }
        public virtual Inventory Inventory { get; set; } = null!;
        public Guid? ExportNoteTo { get; private set; }
        public virtual ServiceCenter? ExportNoteToCenter { get; set; }
        public Guid? ExportNoteFrom { get; private set; }
        public virtual ServiceCenter? ExportNoteFromCenter { get; set; }
        public ExportType ExportType { get; private set; }
        public string? ReferenceType { get; private set; }
        public Guid? ApproveBy { get; private set; }
        public virtual Staff? ApprovedByStaff { get; set; }
        public ExportStatus Status { get; private set; }
        public string? Note { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }

        private ExportNote()
        {
            // For EF Core
        }

        public ExportNote(
            Guid id,
            Guid createBy,
            Guid inventoryId,
            ExportType exportType,
            Guid? exportNoteTo = null,
            Guid? exportNoteFrom = null,
            string? referenceType = null,
            string? note = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            CreateBy = ValidateGuid(createBy, nameof(createBy));
            InventoryId = ValidateGuid(inventoryId, nameof(inventoryId));

            ExportType = exportType;
            ExportNoteTo = ValidateNullableGuid(exportNoteTo, nameof(exportNoteTo));
            ExportNoteFrom = ValidateNullableGuid(exportNoteFrom, nameof(exportNoteFrom));
            ReferenceType = NormalizeNullable(referenceType);
            Note = NormalizeNullable(note);

            Status = ExportStatus.PENDING;
        }

        public void ChangeRoute(Guid? exportNoteFrom, Guid? exportNoteTo)
        {
            ExportNoteFrom = ValidateNullableGuid(exportNoteFrom, nameof(exportNoteFrom));
            ExportNoteTo = ValidateNullableGuid(exportNoteTo, nameof(exportNoteTo));
            Touch();
        }

        public void ChangeReferenceType(string? referenceType)
        {
            ReferenceType = NormalizeNullable(referenceType);
            Touch();
        }

        public void ChangeNote(string? note)
        {
            Note = NormalizeNullable(note);
            Touch();
        }

        public void Confirm(Guid approveBy, DateTime? confirmedAtUtc = null)
        {
            ApproveBy = ValidateGuid(approveBy, nameof(approveBy));
            ConfirmedAt = confirmedAtUtc ?? DateTime.UtcNow;
            Status = ExportStatus.CONFIRMED;
            Touch();
        }

        public void MarkExported()
        {
            EnsureConfirmed();
            Status = ExportStatus.EXPORTED;
            Touch();
        }

        public void MarkInTransit()
        {
            EnsureConfirmed();
            Status = ExportStatus.INTRANSIT;
            Touch();
        }

        public void MarkDelivered()
        {
            EnsureConfirmed();
            Status = ExportStatus.DELIVERED;
            Touch();
        }

        public void SetStatus(ExportStatus status)
        {
            Status = status;
            if (status == ExportStatus.CONFIRMED && !ConfirmedAt.HasValue)
                ConfirmedAt = DateTime.UtcNow;

            Touch();
        }

        private void EnsureConfirmed()
        {
            if (Status == ExportStatus.PENDING)
                throw new InvalidOperationException("ExportNote must be confirmed before this transition.");
        }

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

        private static string? NormalizeNullable(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }
    }
}
