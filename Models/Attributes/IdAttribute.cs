using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Models.Db.Common;

namespace Models.Attributes
{
    public class IdAttribute : ValidationAttribute
    {
        private readonly Type _entityType;

        public IdAttribute(Type entityType)
        {
            if (!entityType.IsAssignableTo(typeof(IdEntity)))
            {
                throw new($"Invalid Type Provided To IdAttribute: {entityType.Name}! Derivative of IdEntity Is Required");
            }

            _entityType = entityType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult($"{_entityType.Name}: Provided Value Was null");
            }

            var checker = validationContext.GetRequiredService<Func<Type, object, bool>>();

            return checker(_entityType, value)
                ? ValidationResult.Success
                : new ValidationResult($"{_entityType.Name} with Id: {value} is not found");
        }
    }
}