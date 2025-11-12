namespace TwinsWins.Core.Entities;

public class ImagePair : BaseEntity
{
    public Guid SetId { get; set; }
    public ImageSet Set { get; set; } = null!;
    public string Image1Path { get; set; } = string.Empty;
    public string Image2Path { get; set; } = string.Empty;
    public string? LogicDescription { get; set; }
    public int DifficultyRating { get; set; } = 5;
    public int UsageCount { get; set; } = 0;
}
