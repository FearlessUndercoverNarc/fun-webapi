using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class LargerThanAttribute : ValidationAttribute
    {
        private readonly string _propertyName;

        public LargerThanAttribute(string propertyName)
        {
            _propertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();

            // ReSharper disable once PossibleNullReferenceException
            var comparedValue = type.GetProperty(_propertyName).GetValue(instance, null);

            if (value is not IComparable comparable)
            {
                throw new("LargerThanAttribute, provided value was not an instance of IComparable");
            }

            return comparable.CompareTo(comparedValue) > 0 ? ValidationResult.Success : new ValidationResult($"Value must be larger than {_propertyName}");
        }
    }
}