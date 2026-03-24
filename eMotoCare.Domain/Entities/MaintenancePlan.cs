using eMotoCare.Domain.Base;
using eMotoCare.Domain.Base.Common;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class MaintenancePlan : BaseEntity
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public int TotalStage { get; private set; }
        public string? Description { get; private set; }
        public Status Status { get; private set; }

        private MaintenancePlan()
        {
            // For EF Core
        }

        public MaintenancePlan(
            Guid id,
            string code,
            string name,
            int totalStage,
            string? description = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();
            Name = ValidateRequired(name, nameof(name));
            TotalStage = ValidateTotalStage(totalStage);
            Description = NormalizeNullable(description);
            Status = Status.ACTIVE;
        }

        public static MaintenancePlan CreateWithGeneratedCode(
            Guid id,
            string codeSeed,
            Func<string, bool> existsCodeInMaintenancePlanTable,
            string name,
            int totalStage,
            string? description = null)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInMaintenancePlanTable);

            return new MaintenancePlan(
                id,
                generatedCode,
                name,
                totalStage,
                description);
        }

        public void ChangeName(string name)
        {
            Name = ValidateRequired(name, nameof(name));
            Touch();
        }

        public void ChangeTotalStage(int totalStage)
        {
            TotalStage = ValidateTotalStage(totalStage);
            Touch();
        }

        public void ChangeDescription(string? description)
        {
            Description = NormalizeNullable(description);
            Touch();
        }

        public void SetStatus(Status status)
        {
            Status = status;
            Touch();
        }

        private static int ValidateTotalStage(int totalStage)
        {
            if (totalStage <= 0)
                throw new ArgumentOutOfRangeException(nameof(totalStage), "TotalStage must be greater than 0.");

            return totalStage;
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
