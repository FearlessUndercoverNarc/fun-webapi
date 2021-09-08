namespace Models.DTOs.Misc
{
    public class IdDto
    {
        public long Id { get; set; }

        public IdDto()
        {
        }

        public IdDto(long id)
        {
            Id = id;
        }

        public static implicit operator IdDto(long id)
        {
            return new(id);
        }

        public static implicit operator long(IdDto idDto)
        {
            return idDto.Id;
        }
    }
}