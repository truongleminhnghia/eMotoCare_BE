using eMotoCare.Domain.Base;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class RMADetail : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid PartItemId { get; private set; }
        public virtual PartItem PartItem { get; set; } = null!;

        public Guid EVCheckDetailId { get; private set; }
        public virtual EVCheckDetail EVCheckDetail { get; set; } = null!;

        public Guid RMAId { get; private set; }
        public virtual RMA RMA { get; set; } = null!;

        public RMASolution Solution { get; private set; }
        public int Quantity { get; private set; }
        public RMADetailStatus Status { get; private set; }
        public RMACondition Condition { get; private set; }

        public string IssueDescription { get; private set; } = null!;

        public Guid? ReplacementPartItemId { get; private set; }
        public virtual PartItem? ReplacementPartItem { get; set; }

        public string? Note { get; private set; }

        private RMADetail()
        {
            // For EF Core
        }

        public RMADetail(
            Guid id,
            Guid partItemId,
            Guid evCheckDetailId,
            Guid rmaId,
            RMASolution solution,
            int quantity,
            RMACondition condition,
            string issueDescription,
            Guid? replacementPartItemId = null,
            string? note = null)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            PartItemId = ValidateGuid(partItemId, nameof(partItemId));
            EVCheckDetailId = ValidateGuid(evCheckDetailId, nameof(evCheckDetailId));
            RMAId = ValidateGuid(rmaId, nameof(rmaId));
            ReplacementPartItemId = ValidateNullableGuid(replacementPartItemId, nameof(replacementPartItemId));

            Solution = solution;
            Quantity = ValidateQuantity(quantity);
            Status = RMADetailStatus.PENDING;
            Condition = condition;

            IssueDescription = ValidateRequired(issueDescription, nameof(issueDescription));
            Note = NormalizeNullable(note);
        }

        public void ChangeSolution(RMASolution solution)
        {
            Solution = solution;
            Touch();
        }

        public void ChangeQuantity(int quantity)
        {
            Quantity = ValidateQuantity(quantity);
            Touch();
        }

        public void ChangeCondition(RMACondition condition)
        {
            Condition = condition;
            Touch();
        }

        public void ChangeIssueDescription(string issueDescription)
        {
            IssueDescription = ValidateRequired(issueDescription, nameof(issueDescription));
            Touch();
        }

        public void ChangeReplacementPartItem(Guid? replacementPartItemId)
        {
            ReplacementPartItemId = ValidateNullableGuid(replacementPartItemId, nameof(replacementPartItemId));
            Touch();
        }

        public void ChangeNote(string? note)
        {
            Note = NormalizeNullable(note);
            Touch();
        }

        public void MarkInspected()
        {
            if (Status != RMADetailStatus.PENDING)
                throw new InvalidOperationException("Only PENDING details can be marked as INSPECTED.");

            Status = RMADetailStatus.INSPECTED;
            Touch();
        }

        public void Approve()
        {
            if (Status != RMADetailStatus.INSPECTED)
                throw new InvalidOperationException("Only INSPECTED details can be approved.");

            Status = RMADetailStatus.APPROVED;
            Touch();
        }

        public void Reject()
        {
            if (Status != RMADetailStatus.INSPECTED)
                throw new InvalidOperationException("Only INSPECTED details can be rejected.");

            Status = RMADetailStatus.REJECTED;
            Touch();
        }

        public void MarkResolved()
        {
            if (Status != RMADetailStatus.APPROVED)
                throw new InvalidOperationException("Only APPROVED details can be marked as RESOLVED.");

            Status = RMADetailStatus.RESOLVED;
            Touch();
        }

        public void MarkDisputed()
        {
            if (Status == RMADetailStatus.RESOLVED || Status == RMADetailStatus.DISPUTED)
                throw new InvalidOperationException("Cannot dispute resolved or already disputed details.");

            Status = RMADetailStatus.DISPUTED;
            Touch();
        }

        public void SetStatus(RMADetailStatus status)
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

        private static Guid? ValidateNullableGuid(Guid? value, string fieldName)
        {
            if (value.HasValue && value.Value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty Guid.", fieldName);

            return value;
        }

        private static int ValidateQuantity(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Quantity must be greater than 0.");

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
