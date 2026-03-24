using eMotoCare.Domain.Base;
using eMotoCare.Domain.Base.Common;
using eMotoCare.Domain.Enums;
namespace eMotoCare.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid AccountId { get; private set; }
        public virtual Account Account { get; set; } = null!;

        public string Code { get; private set; } = null!;
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public Gender Gender { get; private set; }
        public string Address { get; private set; } = null!;
        public Status Status { get; private set; }

        private Customer()
        {
            // For EF Core
        }

        public Customer(
            Guid id,
            Guid accountId,
            string code,
            string firstName,
            string lastName,
            Gender gender,
            string address)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            AccountId = ValidateGuid(accountId, nameof(accountId));
            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();
            FirstName = ValidateRequired(firstName, nameof(firstName));
            LastName = ValidateRequired(lastName, nameof(lastName));
            Gender = gender;
            Address = ValidateRequired(address, nameof(address));
            Status = Status.ACTIVE;
        }

        public static Customer CreateWithGeneratedCode(
            Guid id,
            Guid accountId,
            Guid memberGroupId,
            string codeSeed,
            Func<string, bool> existsCodeInCustomerTable,
            string firstName,
            string lastName,
            Gender gender,
            string address)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInCustomerTable);

            return new Customer(
                id,
                accountId,
                generatedCode,
                firstName,
                lastName,
                gender,
                address);
        }

        public void ChangeName(string firstName, string lastName)
        {
            FirstName = ValidateRequired(firstName, nameof(firstName));
            LastName = ValidateRequired(lastName, nameof(lastName));
            Touch();
        }

        public void ChangeGender(Gender gender)
        {
            Gender = gender;
            Touch();
        }

        public void ChangeAddress(string address)
        {
            Address = ValidateRequired(address, nameof(address));
            Touch();
        }

        public void ChangeAccount(Guid accountId)
        {
            AccountId = ValidateGuid(accountId, nameof(accountId));
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
