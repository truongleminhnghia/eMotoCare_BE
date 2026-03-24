using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class VehiclePartItem : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid VehicleId { get; private set; }
        public virtual Vehicle Vehicle { get; set; } = null!;

        public Guid PartItemId { get; private set; }
        public virtual PartItem PartItem { get; set; } = null!;

        public DateTime InstalledAt { get; private set; }
        public DateTime? RemovedAt { get; private set; }

        public int MileageAtInstall { get; private set; }
        public int? MileageAtRemove { get; private set; }

        public DateOnly? WarrantyStartDate { get; private set; }
        public DateOnly? WarrantyEndDate { get; private set; }

        public VehiclePartItemStatus Status { get; private set; }

        public Guid InstalledBy { get; private set; }
        public virtual Staff InstalledByStaff { get; set; } = null!;

        public string? Note { get; private set; }

        private VehiclePartItem()
        {
            // For EF Core
        }

        public VehiclePartItem(
            Guid id,
            Guid vehicleId,
            Guid partItemId,
            DateTime installedAt,
            int mileageAtInstall,
            Guid installedBy,
            DateOnly? warrantyStartDate = null,
            DateOnly? warrantyEndDate = null,
            string? note = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            VehicleId = ValidateGuid(vehicleId, nameof(vehicleId));
            PartItemId = ValidateGuid(partItemId, nameof(partItemId));
            InstalledBy = ValidateGuid(installedBy, nameof(installedBy));

            InstalledAt = installedAt;
            MileageAtInstall = ValidateMileage(mileageAtInstall);

            SetWarranty(warrantyStartDate, warrantyEndDate);

            Note = NormalizeNullable(note);
            Status = VehiclePartItemStatus.INSTALLED;
        }

        public void Remove(DateTime removedAt, int mileageAtRemove, string? note = null)
        {
            if (removedAt < InstalledAt)
                throw new ArgumentException("RemovedAt must be greater than or equal to InstalledAt.", nameof(removedAt));

            if (mileageAtRemove < MileageAtInstall)
                throw new ArgumentException("MileageAtRemove must be greater than or equal to MileageAtInstall.", nameof(mileageAtRemove));

            RemovedAt = removedAt;
            MileageAtRemove = mileageAtRemove;
            Note = NormalizeNullable(note) ?? Note;
            Status = VehiclePartItemStatus.REMOVED;
            Touch();
        }

        public void ChangeInstalledBy(Guid installedBy)
        {
            InstalledBy = ValidateGuid(installedBy, nameof(installedBy));
            Touch();
        }

        public void ChangeNote(string? note)
        {
            Note = NormalizeNullable(note);
            Touch();
        }

        public void SetStatus(VehiclePartItemStatus status)
        {
            Status = status;
            Touch();
        }

        public void SetWarranty(DateOnly? warrantyStartDate, DateOnly? warrantyEndDate)
        {
            if (warrantyStartDate.HasValue != warrantyEndDate.HasValue)
                throw new ArgumentException("WarrantyStartDate and WarrantyEndDate must both be null or both have values.");

            if (warrantyStartDate.HasValue && warrantyEndDate!.Value < warrantyStartDate.Value)
                throw new ArgumentException("WarrantyEndDate must be greater than or equal to WarrantyStartDate.");

            WarrantyStartDate = warrantyStartDate;
            WarrantyEndDate = warrantyEndDate;
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
