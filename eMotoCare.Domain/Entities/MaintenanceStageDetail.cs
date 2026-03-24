using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class MaintenanceStageDetail : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid PartId { get; private set; }
        public virtual Part Part { get; set; } = null!;

        public Guid StageId { get; private set; }
        public virtual MaintenanceStage Stage { get; set; } = null!;

        public MaintenanceAction Action { get; private set; }
        public string? Description { get; private set; }

        private MaintenanceStageDetail()
        {
            // For EF Core
        }

        public MaintenanceStageDetail(
            Guid id,
            Guid partId,
            Guid stageId,
            MaintenanceAction action,
            string? description = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            PartId = ValidateGuid(partId, nameof(partId));
            StageId = ValidateGuid(stageId, nameof(stageId));

            Action = action;
            Description = NormalizeNullable(description);
        }

        public void ChangeAction(MaintenanceAction action)
        {
            Action = action;
            Touch();
        }

        public void ChangeDescription(string? description)
        {
            Description = NormalizeNullable(description);
            Touch();
        }

        public void ChangePart(Guid partId)
        {
            PartId = ValidateGuid(partId, nameof(partId));
            Touch();
        }

        public void ChangeStage(Guid stageId)
        {
            StageId = ValidateGuid(stageId, nameof(stageId));
            Touch();
        }

        private static Guid ValidateGuid(Guid value, string fieldName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value;
        }

        private static string? NormalizeNullable(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Trim();
        }
    }
}
