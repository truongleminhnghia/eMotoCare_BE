using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class PartItem : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid PartId { get; private set; }
        public virtual Part Part { get; set; } = null!;

        public Guid InventoryId { get; private set; }
        public virtual Inventory Inventory { get; set; } = null!;

        public string? SerialNumber { get; private set; }
        public string? BatchNumber { get; private set; }

        public decimal ImportPrice { get; private set; }

        public bool IsWarranty { get; private set; }
        public int? WarrantyTime { get; private set; } // month
        public DateOnly? WarrantyStartDate { get; private set; }
        public DateOnly? WarrantyEndDate { get; private set; }

        public PartItemStatus Status { get; private set; }

        public Guid LocationId { get; private set; }
        public virtual InventoryLocation Location { get; set; } = null!;

        public int Quantity { get; private set; }

        private PartItem()
        {
            // For EF Core
        }

        public PartItem(
            Guid id,
            Guid partId,
            Guid inventoryId,
            Guid locationId,
            decimal importPrice,
            int quantity,
            bool isWarranty,
            int? warrantyTime = null,
            DateOnly? warrantyStartDate = null,
            DateOnly? warrantyEndDate = null,
            string? serialNumber = null,
            string? batchNumber = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            PartId = ValidateGuid(partId, nameof(partId));
            InventoryId = ValidateGuid(inventoryId, nameof(inventoryId));
            LocationId = ValidateGuid(locationId, nameof(locationId));

            ImportPrice = ValidateMoney(importPrice, nameof(importPrice));
            Quantity = ValidateQuantity(quantity);

            SerialNumber = NormalizeNullable(serialNumber);
            BatchNumber = NormalizeNullable(batchNumber);

            SetWarranty(isWarranty, warrantyTime, warrantyStartDate, warrantyEndDate);

            Status = PartItemStatus.IN_STOCK;
        }

        public void ChangeLocation(Guid locationId)
        {
            LocationId = ValidateGuid(locationId, nameof(locationId));
            Touch();
        }

        public void ChangeInventory(Guid inventoryId)
        {
            InventoryId = ValidateGuid(inventoryId, nameof(inventoryId));
            Touch();
        }

        public void ChangePart(Guid partId)
        {
            PartId = ValidateGuid(partId, nameof(partId));
            Touch();
        }

        public void ChangeSerialNumber(string? serialNumber)
        {
            SerialNumber = NormalizeNullable(serialNumber);
            Touch();
        }

        public void ChangeBatchNumber(string? batchNumber)
        {
            BatchNumber = NormalizeNullable(batchNumber);
            Touch();
        }

        public void ChangeImportPrice(decimal importPrice)
        {
            ImportPrice = ValidateMoney(importPrice, nameof(importPrice));
            Touch();
        }

        public void ChangeQuantity(int quantity)
        {
            Quantity = ValidateQuantity(quantity);
            Touch();
        }

        public void SetStatus(PartItemStatus status)
        {
            Status = status;
            Touch();
        }

        public void SetWarranty(
            bool isWarranty,
            int? warrantyTime = null,
            DateOnly? warrantyStartDate = null,
            DateOnly? warrantyEndDate = null)
        {
            IsWarranty = isWarranty;

            if (!isWarranty)
            {
                WarrantyTime = null;
                WarrantyStartDate = null;
                WarrantyEndDate = null;
                Touch();
                return;
            }

            if (!warrantyTime.HasValue || warrantyTime.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(warrantyTime), "WarrantyTime must be greater than 0.");

            if (!warrantyStartDate.HasValue || !warrantyEndDate.HasValue)
                throw new ArgumentException("WarrantyStartDate and WarrantyEndDate are required when IsWarranty is true.");

            if (warrantyEndDate.Value < warrantyStartDate.Value)
                throw new ArgumentException("WarrantyEndDate must be greater than or equal to WarrantyStartDate.");

            WarrantyTime = warrantyTime.Value;
            WarrantyStartDate = warrantyStartDate.Value;
            WarrantyEndDate = warrantyEndDate.Value;

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

        private static int ValidateQuantity(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Quantity must be greater than 0.");

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
