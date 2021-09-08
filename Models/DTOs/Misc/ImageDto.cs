namespace Models.DTOs.Misc
{
    public class ImageDto
    {
        public string Image { get; set; }

        public ImageDto(string image)
        {
            Image = image;
        }
    }
}