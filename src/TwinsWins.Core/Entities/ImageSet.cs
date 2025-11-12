using TwinsWins.Core.Enums;

namespace TwinsWins.Core.Entities;

public class ImageSet : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPremium { get; set; } = false;
    public string ImageStoragePath { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<ImagePair> ImagePairs { get; set; } = new List<ImagePair>();
}
