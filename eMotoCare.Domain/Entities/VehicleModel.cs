using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class VehicleModel : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid MaintenancePlanId { get; private set; }
        public virtual MaintenancePlan MaintenancePlan { get; set; } = null!;

        public string Name { get; private set; } = null!;
        public Status Status { get; private set; }

        private VehicleModel()
        {
            // For EF Core
        }

        public VehicleModel(Guid id, Guid maintenancePlanId, string name)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            MaintenancePlanId = ValidateGuid(maintenancePlanId, nameof(maintenancePlanId));
            Name = ValidateRequired(name, nameof(name));
            Status = Status.ACTIVE;
        }

        public void ChangeName(string name)
        {
            Name = ValidateRequired(name, nameof(name));
            Touch();
        }

        public void ChangeMaintenancePlan(Guid maintenancePlanId)
        {
            MaintenancePlanId = ValidateGuid(maintenancePlanId, nameof(maintenancePlanId));
            Touch();
        }

        public void SetStatus(Status status)
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

        private static string ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value.Trim();
        }
    }
}
