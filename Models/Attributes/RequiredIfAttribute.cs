using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class RequiredIfAttribute : RequiredAttribute
    {
        private readonly string _propertyName;
        private readonly object _desiredValue;

        public RequiredIfAttribute(string propertyName, object desiredValue)
        {
            _propertyName = propertyName;
            _desiredValue = desiredValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();

            // ReSharper disable once PossibleNullReferenceException
            var propertyValue = type.GetProperty(_propertyName).GetValue(instance, null);

            return propertyValue == _desiredValue ? base.IsValid(value, context) : ValidationResult.Success;
        }
    }
}