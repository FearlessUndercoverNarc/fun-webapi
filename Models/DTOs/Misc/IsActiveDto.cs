namespace Models.DTOs.Misc
{
    public class IsActiveDto
    {
        public bool IsActive { get; set; }

        public IsActiveDto(bool isActive)
        {
            IsActive = isActive;
        }

        public static implicit operator IsActiveDto(bool value)
        {
            return new(value);
        }
        
        public static implicit operator bool(IsActiveDto value)
        {
            return value.IsActive;
        }
    }
}