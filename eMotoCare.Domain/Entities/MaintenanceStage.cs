using eMotoCare.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class MaintenanceStage : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid MaintenancePlanId { get; private set; }
        public virtual MaintenancePlan MaintenancePlan { get; set; } = null!;

        public int StageNumber { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public int Mileage { get; private set; }
        public bool IsRequired { get; private set; }

        private MaintenanceStage()
        {
            // For EF Core
        }

        public MaintenanceStage(
            Guid id,
            Guid maintenancePlanId,
            int stageNumber,
            string name,
            int mileage,
            bool isRequired,
            string? description = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            MaintenancePlanId = ValidateGuid(maintenancePlanId, nameof(maintenancePlanId));
            StageNumber = ValidateStageNumber(stageNumber);
            Name = ValidateRequired(name, nameof(name));
            Mileage = ValidateMileage(mileage);
            IsRequired = isRequired;
            Description = NormalizeNullable(description);
        }

        public void ChangeStageNumber(int stageNumber)
        {
            StageNumber = ValidateStageNumber(stageNumber);
            Touch();
        }

        public void ChangeName(string name)
        {
            Name = ValidateRequired(name, nameof(name));
            Touch();
        }

        public void ChangeDescription(string? description)
        {
            Description = NormalizeNullable(description);
            Touch();
        }

        public void ChangeMileage(int mileage)
        {
            Mileage = ValidateMileage(mileage);
            Touch();
        }

        public void SetRequired(bool isRequired)
        {
            IsRequired = isRequired;
            Touch();
        }

        public void ChangeMaintenancePlan(Guid maintenancePlanId)
        {
            MaintenancePlanId = ValidateGuid(maintenancePlanId, nameof(maintenancePlanId));
            Touch();
        }

        private static Guid ValidateGuid(Guid value, string fieldName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value;
        }

        private static int ValidateStageNumber(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "StageNumber must be greater than 0.");

            return value;
        }

        private static int ValidateMileage(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Mileage cannot be negative.");

            return value;
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
