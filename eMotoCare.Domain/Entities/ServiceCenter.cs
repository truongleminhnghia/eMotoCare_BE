using eMotoCare.Domain.Base;
using eMotoCare.Domain.Base.Common;
using eMotoCare.Domain.Enums;
using eMotoCare.Domain.ValueObjects.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class ServiceCenter : BaseEntity
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; } = null!;
        public PhoneNumber Phone { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public string Address { get; private set; } = null!;
        public TimeOnly OpeningTime { get; private set; }
        public TimeOnly ClosingTime { get; private set; }
        public Status Status { get; private set; }

        public ServiceCenter()
        {
            
        }

        public ServiceCenter(
            Guid id,
            string code,
            PhoneNumber phone,
            Email email,
            string address,
            TimeOnly openingTime,
            TimeOnly closingTime)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Address = ValidateRequired(address, nameof(address));
            SetBusinessHours(openingTime, closingTime);

            Status = Status.ACTIVE;
        }

        public static ServiceCenter CreateWithGeneratedCode(
            Guid id,
            string codeSeed,
            Func<string, bool> existsCodeInServiceCenterTable,
            PhoneNumber phone,
            Email email,
            string address,
            TimeOnly openingTime,
            TimeOnly closingTime)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInServiceCenterTable);

            return new ServiceCenter(
                id,
                generatedCode,
                phone,
                email,
                address,
                openingTime,
                closingTime);
        }

        public void ChangeContact(PhoneNumber phone, Email email)
        {
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Touch();
        }

        public void ChangeAddress(string address)
        {
            Address = ValidateRequired(address, nameof(address));
            Touch();
        }

        public void ChangeBusinessHours(TimeOnly openingTime, TimeOnly closingTime)
        {
            SetBusinessHours(openingTime, closingTime);
            Touch();
        }

        public void SetStatus(Status status)
        {
            Status = status;
            Touch();
        }

        private void SetBusinessHours(TimeOnly openingTime, TimeOnly closingTime)
        {
            if (openingTime >= closingTime)
                throw new ArgumentException("OpeningTime must be earlier than ClosingTime.");

            OpeningTime = openingTime;
            ClosingTime = closingTime;
        }

        private static string ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value.Trim();
        }
    }
}