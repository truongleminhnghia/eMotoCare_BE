using eMotoCare.Domain.Base;
using eMotoCare.Domain.Base.Common;

namespace eMotoCare.Domain.Entities
{
    public class PartType : BaseEntity
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        private PartType()
        {
            // For EF Core
        }

        public PartType(
            Guid id,
            string code,
            string name,
            string? description = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();
            Name = ValidateRequired(name, nameof(name));
            Description = NormalizeNullable(description);
        }

        public static PartType CreateWithGeneratedCode(
            Guid id,
            string codeSeed,
            Func<string, bool> existsCodeInPartTypeTable,
            string name,
            string? description = null)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInPartTypeTable);

            return new PartType(
                id,
                generatedCode,
                name,
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
