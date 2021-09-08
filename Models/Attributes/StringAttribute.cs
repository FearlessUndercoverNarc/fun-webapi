using System.ComponentModel.DataAnnotations;

namespace Models.Attributes
{
    public class StringAttribute : StringLengthAttribute
    {
        public StringAttribute(int minimumLength, int maximumLength) : base(maximumLength)
        {
            MinimumLength = minimumLength;
        }
    }
}