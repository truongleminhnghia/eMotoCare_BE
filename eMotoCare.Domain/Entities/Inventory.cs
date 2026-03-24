using eMotoCare.Domain.Base;

namespace eMotoCare.Domain.Entities
{
    public class Inventory : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid ServiceCenterId { get; private set; }
        public virtual ServiceCenter ServiceCenter { get; set; } = null!;

        public Guid WarehouseManagerId { get; private set; }
        public virtual Staff WarehouseManager { get; set; } = null!;

        public string Address { get; private set; } = null!;

        private Inventory()
        {
            // For EF Core
        }

        public Inventory(
            Guid id,
            Guid serviceCenterId,
            Guid warehouseManagerId,
            string address)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            ServiceCenterId = ValidateGuid(serviceCenterId, nameof(serviceCenterId));
            WarehouseManagerId = ValidateGuid(warehouseManagerId, nameof(warehouseManagerId));
            Address = ValidateRequired(address, nameof(address));
        }

        public void ChangeAddress(string address)
        {
            Address = ValidateRequired(address, nameof(address));
            Touch();
        }

        public void ChangeServiceCenter(Guid serviceCenterId)
        {
            ServiceCenterId = ValidateGuid(serviceCenterId, nameof(serviceCenterId));
            Touch();
        }

        public void ChangeWarehouseManager(Guid warehouseManagerId)
        {
            WarehouseManagerId = ValidateGuid(warehouseManagerId, nameof(warehouseManagerId));
            Touch();
        }

        private static Guid ValidateGuid(Guid value, string fieldName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value;
        }

        private static string ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value.Trim();
        }
    }
}
