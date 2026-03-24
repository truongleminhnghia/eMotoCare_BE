using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using eMotoCare.Domain.ValueObjects.Commons;

namespace eMotoCare.Domain.Entities
{
    public class Account : BaseEntity
    {
        public Guid Id { get; private set; }

        public PhoneNumber? Phone { get; private set; } = null!;
        public Email? Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;

        public AccountStatus Status { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public int FailedLoginAttempts { get; private set; }
        public bool Locked { get; private set; }

        private Account()
        {
            // For EF Core
        }

        public Account(Guid id, PhoneNumber phone, Email email, string passwordHash)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = ValidatePasswordHash(passwordHash);

            Status = AccountStatus.INACTIVE;
            FailedLoginAttempts = 0;
            Locked = false;

            var now = DateTime.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
        }

        public void ChangePhone(PhoneNumber newPhone)
        {
            Phone = newPhone ?? throw new ArgumentNullException(nameof(newPhone));
            Touch();
        }

        public void ChangeEmail(Email newEmail)
        {
            Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
            Touch();
        }

        public void ChangePasswordHash(string newPasswordHash)
        {
            PasswordHash = ValidatePasswordHash(newPasswordHash);
            Touch();
        }

        public void MarkLoginSucceeded()
        {
            LastLoginAt = DateTime.UtcNow;
            FailedLoginAttempts = 0;
            Locked = false;
            Touch();
        }

        public void MarkLoginFailed(int maxFailedAttempts = 5)
        {
            if (maxFailedAttempts <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxFailedAttempts));

            FailedLoginAttempts++;

            if (FailedLoginAttempts >= maxFailedAttempts)
                Locked = true;

            Touch();
        }

        public void Lock()
        {
            Locked = true;
            Touch();
        }

        public void Unlock()
        {
            Locked = false;
            FailedLoginAttempts = 0;
            Touch();
        }

        public void SetStatus(AccountStatus status)
        {
            Status = status;
            Touch();
        }

        private static string ValidatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash cannot be empty.", nameof(passwordHash));

            return passwordHash.Trim();
        }
    }
}
