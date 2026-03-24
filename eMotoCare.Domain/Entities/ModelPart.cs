using eMotoCare.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class ModelPart : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid VehicleModelId { get; private set; }
        public virtual VehicleModel VehicleModel { get; set; } = null!;

        public Guid PartId { get; private set; }
        public virtual Part Part { get; set; } = null!;

        public bool IsRequired { get; private set; }
        public int ReplacementIntervalKm { get; private set; }

        public bool IsWarranty { get; private set; }
        public int? TimeWarranty { get; private set; } // month

        private ModelPart()
        {
            // For EF Core
        }

        public ModelPart(
            Guid id,
            Guid vehicleModelId,
            Guid partId,
            bool isRequired,
            int replacementIntervalKm,
            bool isWarranty,
            int? timeWarranty = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            VehicleModelId = ValidateGuid(vehicleModelId, nameof(vehicleModelId));
            PartId = ValidateGuid(partId, nameof(partId));

            IsRequired = isRequired;
            ReplacementIntervalKm = ValidateReplacementIntervalKm(replacementIntervalKm);
            SetWarranty(isWarranty, timeWarranty);
        }

        public void ChangeVehicleModel(Guid vehicleModelId)
        {
            VehicleModelId = ValidateGuid(vehicleModelId, nameof(vehicleModelId));
            Touch();
        }

        public void ChangePart(Guid partId)
        {
            PartId = ValidateGuid(partId, nameof(partId));
            Touch();
        }

        public void SetRequired(bool isRequired)
        {
            IsRequired = isRequired;
            Touch();
        }

        public void ChangeReplacementIntervalKm(int replacementIntervalKm)
        {
            ReplacementIntervalKm = ValidateReplacementIntervalKm(replacementIntervalKm);
            Touch();
        }

        public void SetWarranty(bool isWarranty, int? timeWarranty = null)
        {
            IsWarranty = isWarranty;

            if (!isWarranty)
            {
                TimeWarranty = null;
                Touch();
                return;
            }

            if (!timeWarranty.HasValue || timeWarranty.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(timeWarranty), "TimeWarranty must be greater than 0 when IsWarranty is true.");

            TimeWarranty = timeWarranty.Value;
            Touch();
        }

        private static Guid ValidateGuid(Guid value, string fieldName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value;
        }

        private static int ValidateReplacementIntervalKm(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "ReplacementIntervalKm must be greater than 0.");

            return value;
        }
    }
}
