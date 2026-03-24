using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class InventoryLocation : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid InventoryId { get; private set; }
        public virtual Inventory Inventory { get; set; } = null!;

        public InventoryLocationCode Code { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        private InventoryLocation()
        {
            // For EF Core
        }

        public InventoryLocation(
            Guid id,
            Guid inventoryId,
            InventoryLocationCode code,
            string? description = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            InventoryId = ValidateGuid(inventoryId, nameof(inventoryId));
            Code = code;
            Name = BuildName(code);
            Description = NormalizeNullable(description);
        }

        public void ChangeCode(InventoryLocationCode code)
        {
            Code = code;
            Name = BuildName(code);
            Touch();
        }

        public void ChangeDescription(string? description)
        {
            Description = NormalizeNullable(description);
            Touch();
        }

        public void ChangeInventory(Guid inventoryId)
        {
            InventoryId = ValidateGuid(inventoryId, nameof(inventoryId));
            Touch();
        }

        private static string BuildName(InventoryLocationCode code) => $"Kệ {code}";

        private static Guid ValidateGuid(Guid value, string fieldName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

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
