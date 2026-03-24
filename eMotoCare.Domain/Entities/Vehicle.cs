using eMotoCare.Domain.Base;
using eMotoCare.Domain.Base.Common;
using eMotoCare.Domain.Enums;

namespace eMotoCare.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid VehicleModelId { get; private set; }
        public virtual VehicleModel VehicleModel { get; set; } = null!;

        public Guid CustomerId { get; private set; }
        public virtual Customer Customer { get; set; } = null!;

        public string Code { get; private set; } = null!;
        public string LicensePlate { get; private set; } = null!;
        public string VINNumber { get; private set; } = null!;
        public string EngineNumber { get; private set; } = null!;
        public string? Color { get; private set; }

        public DateOnly PurchaseDate { get; private set; }
        public int Mileage { get; private set; }
        public DateTime? LastServiceAt { get; private set; }

        public Status Status { get; private set; }
        public string? Note { get; private set; }

        private Vehicle()
        {
            // For EF Core
        }

        public Vehicle(
            Guid id,
            Guid vehicleModelId,
            Guid customerId,
            string code,
            string licensePlate,
            string vinNumber,
            string engineNumber,
            DateOnly purchaseDate,
            int mileage,
            string? color = null,
            DateTime? lastServiceAt = null,
            string? note = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            VehicleModelId = ValidateGuid(vehicleModelId, nameof(vehicleModelId));
            CustomerId = ValidateGuid(customerId, nameof(customerId));

            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();
            LicensePlate = ValidateRequired(licensePlate, nameof(licensePlate)).ToUpperInvariant();
            VINNumber = ValidateRequired(vinNumber, nameof(vinNumber)).ToUpperInvariant();
            EngineNumber = ValidateRequired(engineNumber, nameof(engineNumber)).ToUpperInvariant();

            PurchaseDate = ValidatePurchaseDate(purchaseDate);
            Mileage = ValidateMileage(mileage);

            Color = NormalizeNullable(color);
            LastServiceAt = lastServiceAt;
            Note = NormalizeNullable(note);

            Status = Status.ACTIVE;
        }

        public static Vehicle CreateWithGeneratedCode(
            Guid id,
            Guid vehicleModelId,
            Guid customerId,
            string codeSeed,
            Func<string, bool> existsCodeInVehicleTable,
            string licensePlate,
            string vinNumber,
            string engineNumber,
            DateOnly purchaseDate,
            int mileage,
            string? color = null,
            DateTime? lastServiceAt = null,
            string? note = null)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInVehicleTable);

            return new Vehicle(
                id,
                vehicleModelId,
                customerId,
                generatedCode,
                licensePlate,
                vinNumber,
                engineNumber,
                purchaseDate,
                mileage,
                color,
                lastServiceAt,
                note);
        }

        public void ChangeVehicleModel(Guid vehicleModelId)
        {
            VehicleModelId = ValidateGuid(vehicleModelId, nameof(vehicleModelId));
            Touch();
        }

        public void ChangeCustomer(Guid customerId)
        {
            CustomerId = ValidateGuid(customerId, nameof(customerId));
            Touch();
        }

        public void ChangeLicensePlate(string licensePlate)
        {
            LicensePlate = ValidateRequired(licensePlate, nameof(licensePlate)).ToUpperInvariant();
            Touch();
        }

        public void ChangeVinNumber(string vinNumber)
        {
            VINNumber = ValidateRequired(vinNumber, nameof(vinNumber)).ToUpperInvariant();
            Touch();
        }

        public void ChangeEngineNumber(string engineNumber)
        {
            EngineNumber = ValidateRequired(engineNumber, nameof(engineNumber)).ToUpperInvariant();
            Touch();
        }

        public void ChangeColor(string? color)
        {
            Color = NormalizeNullable(color);
            Touch();
        }

        public void ChangePurchaseDate(DateOnly purchaseDate)
        {
            PurchaseDate = ValidatePurchaseDate(purchaseDate);
            Touch();
        }

        public void UpdateMileage(int mileage)
        {
            Mileage = ValidateMileage(mileage);
            Touch();
        }

        public void MarkServiced(DateTime servicedAtUtc)
        {
            LastServiceAt = servicedAtUtc;
            Touch();
        }

        public void ChangeNote(string? note)
        {
            Note = NormalizeNullable(note);
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

        private static int ValidateMileage(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Mileage cannot be negative.");

            return value;
        }

        private static DateOnly ValidatePurchaseDate(DateOnly value)
        {
            if (value > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("PurchaseDate cannot be in the future.", nameof(value));

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
