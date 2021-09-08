namespace Models.DTOs.Misc
{
    public class VersionDto
    {
        public uint Version { get; set; }

        public VersionDto(uint version)
        {
            Version = version;
        }
        
        public static implicit operator VersionDto(uint version)
        {
            return new(version);
        }
        
        public static implicit operator uint(VersionDto versionDto)
        {
            return versionDto.Version;
        }
    }
}