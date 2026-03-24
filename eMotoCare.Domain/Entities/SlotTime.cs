using eMotoCare.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class SlotTime : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid ServiceCenterId { get; private set; }
        public virtual ServiceCenter ServiceCenter { get; set; } = null!;

        public string Name { get; private set; } = null!;
        public string TimeSlot { get; private set; } = null!; // Auto: StartTime-EndTime

        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }

        public Guid CreateBy { get; private set; }
        public virtual Staff CreatedByStaff { get; set; } = null!;

        private SlotTime()
        {
            // For EF Core
        }

        public SlotTime(
            Guid id,
            Guid serviceCenterId,
            string name,
            TimeOnly startTime,
            TimeOnly endTime,
            Guid createBy)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            ServiceCenterId = ValidateGuid(serviceCenterId, nameof(serviceCenterId));
            Name = ValidateRequired(name, nameof(name));
            CreateBy = ValidateGuid(createBy, nameof(createBy));

            SetTimeRange(startTime, endTime);
        }

        public void ChangeName(string name)
        {
            Name = ValidateRequired(name, nameof(name));
            Touch();
        }

        public void ChangeTimeRange(TimeOnly startTime, TimeOnly endTime)
        {
            SetTimeRange(startTime, endTime);
            Touch();
        }

        public void ChangeServiceCenter(Guid serviceCenterId)
        {
            ServiceCenterId = ValidateGuid(serviceCenterId, nameof(serviceCenterId));
            Touch();
        }

        public void ChangeCreatedBy(Guid createBy)
        {
            CreateBy = ValidateGuid(createBy, nameof(createBy));
            Touch();
        }

        private void SetTimeRange(TimeOnly startTime, TimeOnly endTime)
        {
            if (startTime >= endTime)
                throw new ArgumentException("StartTime must be earlier than EndTime.");

            StartTime = startTime;
            EndTime = endTime;
            TimeSlot = BuildTimeSlot(startTime, endTime); // ví dụ: 9:00-10:00
        }

        private static string BuildTimeSlot(TimeOnly startTime, TimeOnly endTime)
            => $"{startTime:HH\\:mm}-{endTime:HH\\:mm}";

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
