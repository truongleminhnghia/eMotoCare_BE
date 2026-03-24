using eMotoCare.Domain.Base;
using eMotoCare.Domain.Base.Common;
using eMotoCare.Domain.Enums;

namespace eMotoCare.Domain.Entities
{
    public class Staff : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }
        public virtual Account Account { get; set; } = null!;
        public Guid CenterId { get; private set; }
        public virtual ServiceCenter Center { get; set; } = null!;

        public string Code { get; private set; } = null!;
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public DateOnly DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public string Address { get; private set; } = null!;
        public StaffPosition Position { get; private set; }
        public Status Status { get; private set; }

        private Staff()
        {
            // For EF Core
        }

        public Staff(
            Guid id,
            Guid accountId,
            Guid centerId,
            string code,
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            Gender gender,
            string address,
            StaffPosition position)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            AccountId = ValidateGuid(accountId, nameof(accountId));
            CenterId = ValidateGuid(centerId, nameof(centerId));

            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();
            FirstName = ValidateRequired(firstName, nameof(firstName));
            LastName = ValidateRequired(lastName, nameof(lastName));
            DateOfBirth = ValidateDateOfBirth(dateOfBirth);
            Gender = gender;
            Address = ValidateRequired(address, nameof(address));
            Position = position;

            Status = Status.ACTIVE;
        }

        public static Staff CreateWithGeneratedCode(
            Guid id,
            Guid accountId,
            Guid centerId,
            string codeSeed,
            Func<string, bool> existsCodeInStaffTable,
            string firstName,
            string lastName,
            DateOnly dateOfBirth,
            Gender gender,
            string address,
            StaffPosition position)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInStaffTable);

            return new Staff(
                id,
                accountId,
                centerId,
                generatedCode,
                firstName,
                lastName,
                dateOfBirth,
                gender,
                address,
                position);
        }

        public void ChangeName(string firstName, string lastName)
        {
            FirstName = ValidateRequired(firstName, nameof(firstName));
            LastName = ValidateRequired(lastName, nameof(lastName));
            Touch();
        }

        public void ChangeDateOfBirth(DateOnly dateOfBirth)
        {
            DateOfBirth = ValidateDateOfBirth(dateOfBirth);
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

        public void ChangePosition(StaffPosition position)
        {
            Position = position;
            Touch();
        }

        public void ChangeCenter(Guid centerId)
        {
            CenterId = ValidateGuid(centerId, nameof(centerId));
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

        private static DateOnly ValidateDateOfBirth(DateOnly value)
        {
            if (value > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("DateOfBirth cannot be in the future.", nameof(value));

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
