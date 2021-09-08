namespace Models.DTOs.Misc
{
    public class CreatedDto
    {
        public long Id { get; set; }

        public CreatedDto(long id)
        {
            Id = id;
        }
        
        public static implicit operator CreatedDto(long id)
        {
            return new(id);
        }
        
        public static implicit operator long(CreatedDto createdDto)
        {
            return createdDto.Id;
        }
    }
}