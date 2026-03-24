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
    public class Package : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid PackageConditionId { get; private set; }
        public virtual PackageCondition PackageCondition { get; set; } = null!;

        public string Code { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        public int Duration { get; private set; }
        public PackageUnit Unit { get; private set; }
        public int MaxUsage { get; private set; }

        public Status Status { get; private set; }
        public decimal Price { get; private set; }

        private Package()
        {
            // For EF Core
        }

        public Package(
            Guid id,
            Guid packageConditionId,
            string code,
            string name,
            int duration,
            PackageUnit unit,
            int maxUsage,
            decimal price,
            string? description = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            PackageConditionId = ValidateGuid(packageConditionId, nameof(packageConditionId));

            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();
            Name = ValidateRequired(name, nameof(name));
            Description = NormalizeNullable(description);

            Duration = ValidateDuration(duration);
            Unit = unit;
            MaxUsage = ValidateMaxUsage(maxUsage);

            Price = ValidateMoney(price, nameof(price));
            Status = Status.ACTIVE;
        }

        public static Package CreateWithGeneratedCode(
            Guid id,
            Guid packageConditionId,
            string codeSeed,
            Func<string, bool> existsCodeInPackageTable,
            string name,
            int duration,
            PackageUnit unit,
            int maxUsage,
            decimal price,
            string? description = null)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInPackageTable);

            return new Package(
                id,
                packageConditionId,
                generatedCode,
                name,
                duration,
                unit,
                maxUsage,
                price,
                description);
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

        public void ChangeDuration(int duration)
        {
            Duration = ValidateDuration(duration);
            Touch();
        }

        public void ChangeUnit(PackageUnit unit)
        {
            Unit = unit;
            Touch();
        }

        public void ChangeMaxUsage(int maxUsage)
        {
            MaxUsage = ValidateMaxUsage(maxUsage);
            Touch();
        }

        public void ChangePrice(decimal price)
        {
            Price = ValidateMoney(price, nameof(price));
            Touch();
        }

        public void ChangePackageCondition(Guid packageConditionId)
        {
            PackageConditionId = ValidateGuid(packageConditionId, nameof(packageConditionId));
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

        private static int ValidateDuration(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Duration must be greater than 0.");

            return value;
        }

        private static int ValidateMaxUsage(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "MaxUsage cannot be negative.");

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
