using eMotoCare.Domain.Base;
using eMotoCare.Domain.Base.Common;
using eMotoCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Entities
{
    public class Part : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid PartTypeId { get; private set; }
        public virtual PartType PartType { get; set; } = null!;

        public string Code { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string Brand { get; private set; } = null!;

        public PartUnit Unit { get; private set; }

        public decimal CostPrice { get; private set; }
        public decimal SalePrice { get; private set; }
        public decimal ProfitMargin { get; private set; } // percent

        public bool IsSerialized { get; private set; }
        public Status Status { get; private set; }
        public string Image { get; private set; } = null!;

        private Part()
        {
            // For EF Core
        }

        public Part(
            Guid id,
            Guid partTypeId,
            string code,
            string name,
            string brand,
            PartUnit unit,
            decimal costPrice,
            decimal salePrice,
            bool isSerialized,
            string image)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            PartTypeId = ValidateGuid(partTypeId, nameof(partTypeId));

            Code = ValidateRequired(code, nameof(code)).ToUpperInvariant();
            Name = ValidateRequired(name, nameof(name));
            Brand = ValidateRequired(brand, nameof(brand));

            Unit = unit;
            SetPrices(costPrice, salePrice);

            IsSerialized = isSerialized;
            Status = Status.ACTIVE;
            Image = image;
        }

        public static Part CreateWithGeneratedCode(
            Guid id,
            Guid partTypeId,
            string codeSeed,
            Func<string, bool> existsCodeInPartTable,
            string name,
            string brand,
            PartUnit unit,
            decimal costPrice,
            decimal salePrice,
            bool isSerialized,
            string image)
        {
            var generatedCode = EntityCodeGenerator.Generate(codeSeed, existsCodeInPartTable);

            return new Part(
                id,
                partTypeId,
                generatedCode,
                name,
                brand,
                unit,
                costPrice,
                salePrice,
                isSerialized, 
                image);
        }

        public void ChangePartType(Guid partTypeId)
        {
            PartTypeId = ValidateGuid(partTypeId, nameof(partTypeId));
            Touch();
        }

        public void ChangeName(string name)
        {
            Name = ValidateRequired(name, nameof(name));
            Touch();
        }

        public void ChangeBrand(string brand)
        {
            Brand = ValidateRequired(brand, nameof(brand));
            Touch();
        }

        public void ChangeUnit(PartUnit unit)
        {
            Unit = unit;
            Touch();
        }

        public void UpdatePrices(decimal costPrice, decimal salePrice)
        {
            SetPrices(costPrice, salePrice);
            Touch();
        }

        public void SetSerialized(bool isSerialized)
        {
            IsSerialized = isSerialized;
            Touch();
        }

        public void SetStatus(Status status)
        {
            Status = status;
            Touch();
        }

        private void SetPrices(decimal costPrice, decimal salePrice)
        {
            CostPrice = ValidateMoney(costPrice, nameof(costPrice));
            SalePrice = ValidateMoney(salePrice, nameof(salePrice));
            ProfitMargin = CalculateProfitMargin(CostPrice, SalePrice);
        }

        public void SetImage(string image)
        {
            Image = image;
            Touch();
        }

        private static decimal CalculateProfitMargin(decimal costPrice, decimal salePrice)
        {
            if (costPrice <= 0m)
                return 0m;

            return Math.Round(((salePrice - costPrice) / costPrice) * 100m, 2);
        }

        private static Guid ValidateGuid(Guid value, string fieldName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value;
        }

        private static decimal ValidateMoney(decimal value, string fieldName)
        {
            if (value < 0m)
                throw new ArgumentOutOfRangeException(fieldName, $"{fieldName} cannot be negative.");

            return Math.Round(value, 2);
        }

        private static string ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} cannot be empty.", fieldName);

            return value.Trim();
        }
    }
}
